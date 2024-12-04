using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class TileGenerator : MonoBehaviour
{
    public static TileGenerator Instance { get; private set; }
    public GameObject tilePrefab;
    public GameObject enemyPrefab;
    public int gridWidth = 100;
    public int gridHeight = 100;
    public int roomWidth = 15;
    public int roomHeight = 15;
    public int numberOfRooms = 8;
    public GameObject[] obstaclePrefabs1x2;
    public GameObject[] obstaclePrefabs2x2;
    public GameObject[] decorationPrefabs;

    private RoomFactory roomFactory;
    private List<Room> rooms;
    public static Dictionary<Vector2Int, Tile> allTiles = new Dictionary<Vector2Int, Tile>();
    private List<Tile> currentPath = new List<Tile>();
    private List<Tile> permanentPath = new List<Tile>();
    private List<Tile> hoverPath = new List<Tile>();
    private List<Tile> clickedPath = new List<Tile>();
    public bool IsInitialized { get; private set; } = false;
    private bool isPlayerTurn = true;
    private bool isEnemyTurn = false;

    public int floorLevel;

    private int enemiesToMove = 0;

    private string[] enemyNames = {
        "AC", "AS", "BD", "BT", "CT", "FO", "GN", "GY", "HO", "KH",
        "MM", "MR", "MV", "NS", "OV", "PL", "RU", "TI", "VD", "VM",
        "WS", "WW", "YD"
    };

    public bool getPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void setPlayerTurn()
    {
        Debug.Log("Player Turn");
        isPlayerTurn = true;
        isEnemyTurn = false;
        EventManager.TriggerEvent("PlayerTurn");
    }

    public bool getEnemyTurn()
    {
        return isEnemyTurn;
    }

    public void setEnemyTurn()
    {
        Debug.Log("Enemy Turn");
        isEnemyTurn = true;
        isPlayerTurn = false;
        RegisterEnemiesInAggroState();
        StartCoroutine(HandleEnemyTurns());
        EventManager.TriggerEvent("EnemyTurn");
    }

    private IEnumerator HandleEnemyTurns()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && (enemy.IsInAggroState() || enemy.IsInAttackState()))
            {
                enemy.TakeTurn();
                yield return new WaitUntil(() => enemy.HasCompletedTurn);
            }
        }
        setPlayerTurn();
    }

    public void RegisterEnemyMove()
    {
        enemiesToMove++;
    }

    public void EnemyMoveCompleted()
    {
        enemiesToMove--;
        if (enemiesToMove <= 0)
        {
            setPlayerTurn();
        }
    }

    public int getEnemyToMove()
    {
        return enemiesToMove;
    }

    public bool AreAllEnemiesMoved()
    {

        return enemiesToMove <= 0;
    }

    void Awake()
    {
        Instance = this;
        GameFacade.Instance.Initialize();
    }

    public void StartGame()
    {
        if (!GameManager.Instance.isStartGamePressed)
        {
            return;
        }

        GameFacade.Instance.StartGame(floorLevel);
    }

    void Update()
    {
        UpdateEnemyCounter();
        if (isEnemyTurn && enemiesToMove <= 0)
        {
            setPlayerTurn();
        }
    }

    public IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void OptimizedEnding()
    {
        foreach (var tile in allTiles.Values)
        {
            tile.GetComponent<Renderer>().material.color = Color.yellow;
        }

    }

    public IEnumerator GenerateAndConnectRooms()
    {
        roomFactory = new RoomFactory(tilePrefab, obstaclePrefabs1x2, obstaclePrefabs2x2, decorationPrefabs, enemyPrefab, gridWidth, gridHeight, roomWidth, roomHeight, numberOfRooms);
        rooms = roomFactory.GenerateRooms();
        // Debug.Log("Rooms generated: " + rooms.Count);
        yield return null;

        roomFactory.ConnectRooms(rooms);
        // Debug.Log("Rooms connected.");

        bool allRoomsConnected = roomFactory.AreAllRoomsConnected();
        // Debug.Log("All rooms connected: " + allRoomsConnected);


        foreach (Room room in rooms)
        {
            foreach (Tile tile in room.Tiles)
            {
                // tile.SetTileType("Normal");  
                allTiles[tile.Position] = tile;
            }
        }

        IsInitialized = true;

        yield return StartCoroutine(MoveColoredTilesAndPlayer());

        roomFactory.GenerateObstaclesDecorationsAndEnemies(rooms);

        if (floorLevel > 0)
        {
            roomFactory.SpawnEnemiesInRooms(rooms);
        }

        // Debug.Log("Obstacles and decorations generated.");
    }


    public void AddTileToAllTiles(Tile tile)
    {
        allTiles[tile.Position] = tile;
    }

    public void SpawnPlayerInRandomRoom()
    {
        Room randomRoom = rooms[Random.Range(0, rooms.Count)];
        Vector2Int centerPosition = randomRoom.GetCenter();
        Debug.Log("Room Center: " + centerPosition);
        Tile centerTile = randomRoom.Tiles[centerPosition.x - randomRoom.Position.x, centerPosition.y - randomRoom.Position.y];
        centerTile.IsCenterTile = true;
        float tileHeight = centerTile.transform.position.y;
        Vector3 newPosition = new Vector3(centerPosition.x, tileHeight + 2f, centerPosition.y);
        Player.Instance.SetInitialPosition(new Vector2Int((int)newPosition.x, (int)newPosition.z));
        setPlayerTurn();
    }

    public void StopPlayerMovement()
    {
        Player.Instance.StopMovement();
        Player.Instance.SetState(new PlayerIdleState(Player.Instance));
    }

    public void FindPathToTile(Tile targetTile)
    {
        StopPlayerMovement();
        Player.Instance.ClearPath();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
            return;
        }

        Player player = playerObject.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found on the player object.");
            return;
        }

        Tile startTile = GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
        if (startTile == null)
        {
            Debug.LogError("Start tile not found.");
            return;
        }

        if (!targetTile.IsWalkable)
        {
            Debug.LogWarning("Target tile is not walkable.");
            return;
        }

        List<Tile> path = FindPath(startTile, targetTile);
        if (path != null)
        {
            // Debug.Log("Path found from " + startTile.Position + " to " + targetTile.Position);
            ClearHoverPathHighlight();

            currentPath = path;
            foreach (Tile tile in path)
            {
                tile.GetComponent<Renderer>().material.color = Color.green;
            }
            List<Vector2Int> pathPositions = new List<Vector2Int>();
            foreach (Tile tile in path)
            {
                pathPositions.Add(tile.Position);
            }
            Player.Instance.SetPath(pathPositions);
        }
        else
        {
            // Debug.LogWarning("No path found from " + startTile.Position + " to " + targetTile.Position);
        }
    }

    public void SetClickedPath()
    {
        clickedPath = new List<Tile>(currentPath);
    }

    public void ClearHoverPathHighlight()
    {
        foreach (var tile in allTiles.Values)
        {
            if (tile != null && !clickedPath.Contains(tile))
            {
                tile.ResetColor();
            }
        }
        hoverPath.Clear();
        currentPath.Clear();
        permanentPath.Clear();
    }

    public void ClearClickPathHighlight()
    {
        foreach (Tile tile in clickedPath)
        {
            if (!IsClickPathTile(tile) && !permanentPath.Contains(tile))
            {
                tile.ResetColor();
            }
        }
        clickedPath.Clear();
    }

    public void HighlightHoverPathToTile(Tile targetTile)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
            return;
        }

        Tile startTile = GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
        if (startTile == null)
        {
            Debug.LogError("Start tile not found.");
            return;
        }

        Debug.Log("Tile Type" + targetTile.Type);

        List<Tile> path = FindPath(startTile, targetTile);
        if (path != null)
        {
            hoverPath = path;
            foreach (Tile tile in path)
            {
                tile.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

    public void HighlightClickPathToTile(Tile targetTile)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
            return;
        }

        Tile startTile = GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
        if (startTile == null)
        {
            Debug.LogError("Start tile not found.");
            return;
        }

        List<Tile> path = FindPath(startTile, targetTile);
        if (path != null)
        {
            clickedPath = path;
            foreach (Tile tile in path)
            {
                tile.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

    public bool IsHoverPathTile(Tile tile)
    {
        return hoverPath.Contains(tile);
    }

    public bool IsClickPathTile(Tile tile)
    {
        return clickedPath.Contains(tile);
    }

    public void ClearPathHighlight()
    {
        ClearHoverPathHighlight();
        ClearClickPathHighlight();
    }

    public bool HasCurrentPath()
    {
        return currentPath.Count > 0;
    }

    public bool HasPermanentPath()
    {
        return permanentPath.Count > 0;
    }

    public List<Tile> FindPath(Tile startTile, Tile targetTile, bool isForEnemy = false)
    {
        HashSet<Tile> closedSet = new HashSet<Tile>();
        HashSet<Tile> openSet = new HashSet<Tile> { startTile };
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float> { [startTile] = 0 };
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float> { [startTile] = Heuristic(startTile, targetTile) };

        Tile lastProgressTile = startTile;

        while (openSet.Count > 0)
        {
            Tile current = GetTileWithLowestFScore(openSet, fScore);
            if (current == targetTile)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Tile neighbor in GetAllNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || !neighbor.IsWalkable)
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector2Int.Distance(current.Position, neighbor.Position);
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, targetTile);

                if (isForEnemy && Vector2Int.Distance(neighbor.Position, targetTile.Position) < Vector2Int.Distance(lastProgressTile.Position, targetTile.Position))
                {
                    lastProgressTile = neighbor;
                }
            }
        }

        return isForEnemy ? ReconstructPath(cameFrom, lastProgressTile) : null;
    }

    private Tile GetTileWithLowestFScore(HashSet<Tile> openSet, Dictionary<Tile, float> fScore)
    {
        Tile lowest = null;
        float lowestScore = float.MaxValue;
        foreach (Tile tile in openSet)
        {
            if (fScore.TryGetValue(tile, out float score) && score < lowestScore)
            {
                lowest = tile;
                lowestScore = score;
            }
        }
        return lowest;
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile> { current };
        while (cameFrom.TryGetValue(current, out current))
        {
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private float Heuristic(Tile a, Tile b)
    {
        return Vector2Int.Distance(a.Position, b.Position);
    }

    private IEnumerable<Tile> GetAllNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            Tile neighbor = GetTileAtPosition(tile.Position + direction);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public IEnumerable<Vector2Int> GetAllTilePositions()
    {
        return allTiles.Keys;
    }

    public void ResetAllTilesColor()
    {
        foreach (var tile in allTiles.Values)
        {
            tile.ResetColor();
        }
    }

    public IEnumerator MoveColoredTilesAndPlayer()
    {
        yield return new WaitUntil(() => IsInitialized);


        Debug.Log("Moving colored tiles and player.");
        Vector2Int newPositionOffset = new Vector2Int(200, 200);
        Dictionary<Vector2Int, Tile> newAllTiles = new Dictionary<Vector2Int, Tile>();

        foreach (var tile in allTiles.Values)
        {
            Vector3 newPosition = new Vector3(tile.Position.x + newPositionOffset.x, tile.transform.position.y, tile.Position.y + newPositionOffset.y);
            tile.transform.position = newPosition;
            Vector2Int newTilePosition = new Vector2Int(tile.Position.x + newPositionOffset.x, tile.Position.y + newPositionOffset.y);
            tile.Position = newTilePosition;

            if (tile.Type == "Obstacle")
            {
                Transform obstacleTransform = tile.transform.Find("Obstacle");
                if (obstacleTransform != null)
                {
                    obstacleTransform.position = new Vector3(obstacleTransform.position.x + newPositionOffset.x, obstacleTransform.position.y, obstacleTransform.position.z + newPositionOffset.y);
                }
            }

            newAllTiles[newTilePosition] = tile;
        }

        allTiles = newAllTiles;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerObject.transform.position = new Vector3(playerObject.transform.position.x + newPositionOffset.x, playerObject.transform.position.y, playerObject.transform.position.z + newPositionOffset.x);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + newPositionOffset.x, Camera.main.transform.position.y, Camera.main.transform.position.z + newPositionOffset.x);

            Tile initialTile = GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
            if (initialTile != null)
            {
                initialTile.SetWalkable(false);
            }
            HighlightCurrentPlayerTile();
        }



        ResetAllTilesColor();
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        allTiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public void SetAllTilesToDark()
    {
        foreach (var tile in allTiles.Values)
        {
            tile.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    public void SetupPlayerLight()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
            return;
        }

        Light playerLight = playerObject.GetComponentInChildren<Light>();
        if (playerLight == null)
        {
            GameObject lightObject = new GameObject("PlayerLight");
            lightObject.transform.SetParent(playerObject.transform);
            lightObject.transform.localPosition = new Vector3(0, 5, 0);
            playerLight = lightObject.AddComponent<Light>();
            playerLight.type = LightType.Point;
            playerLight.range = 10;
            playerLight.intensity = 1;
        }
    }

    public bool IsAnyEnemyInAggroState()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && enemy.IsInAggroState())
            {
                return true;
            }
        }
        return false;
    }

    public void RegisterEnemiesInAggroState()
    {
        enemiesToMove = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && (enemy.IsInAggroState() || enemy.IsInAttackState()))
            {
                enemiesToMove++;
            }
        }

    }

    public void HighlightCurrentPlayerTile()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
            return;
        }

        Tile currentTile = GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
        if (currentTile != null)
        {
            Debug.Log("Highlighting current player tile.");
            currentTile.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    public void ResetTileState(Tile tile)
    {
        if (tile != null)
        {
            tile.IsOccupied = false;
            tile.SetWalkable(true);
        }
    }

    public string GetRandomEnemyName()
    {
        return enemyNames[Random.Range(0, enemyNames.Length)];
    }

    public Color getRandomEnemyType()
    {
        Color[] enemyColors = new Color[] { Color.red, Color.white, Color.yellow };

        return enemyColors[Random.Range(0, enemyColors.Length)];
    }

    public void GenerateSingleRoomWithObstacles()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("TilePrefab is not assigned in the TileGenerator.");
            return;
        }

        roomFactory = new RoomFactory(tilePrefab, obstaclePrefabs1x2, obstaclePrefabs2x2, decorationPrefabs, enemyPrefab, gridWidth, gridHeight, roomWidth, roomHeight, 1);
        rooms = roomFactory.GenerateRooms();
        Room singleRoom = rooms[0];
        Debug.Log("Single room generated at position: " + singleRoom.Position);

        foreach (Tile tile in singleRoom.Tiles)
        {
            allTiles[tile.Position] = tile;
        }

        roomFactory.GenerateObstaclesDecorationsAndEnemies(rooms);

        if (enemyPrefab != null)
        {
            Vector2Int enemyPosition = singleRoom.GetCenter();
            GameObject enemyObject = Object.Instantiate(enemyPrefab, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemyObject.tag = "Enemy";
                Tile enemyTile = TileGenerator.Instance.GetTileAtPosition(enemyPosition);
                if (enemyTile != null)
                {
                    enemyTile.IsOccupied = true;
                    enemyTile.SetWalkable(false);
                }

                enemy.setEnemyAsBoss();
            }
        }
    }


    public void UpdateEnemyCounter()
    {
        int enemyCount = GameObject.FindGameObjectWithTag("Enemy") == null ? 0 : GameObject.FindGameObjectsWithTag("Enemy").Length;
        PlayerUIManager.Instance.setEnemyCounter(enemyCount);
    }

    public int getEnemyCounter()
    {

        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public IEnumerable Delay(int second)
    {
        yield return new WaitForSeconds(second);
    }

    public Room GetRoomContainingTile(Tile tile)
    {
        foreach (Room room in rooms)
        {
            foreach (Tile roomTile in room.Tiles)
            {
                if (roomTile == tile)
                {
                    return room;
                }
            }
        }
        return null;
    }
}