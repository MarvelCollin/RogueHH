using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomFactory
{
    public static RoomFactory Instance { get; private set; } 
    private int gridWidth; 
    private int gridHeight;
    private int roomWidth; 
    private int roomHeight;
    private int numberOfRooms; 
    private GameObject tilePrefab;
    private GameObject obstaclePrefab;
    private List<Room> rooms; 
    private GameObject[] obstaclePrefabs1x2;
    private GameObject[] obstaclePrefabs2x2;
    private GameObject[] decorationPrefabs;
    private GameObject enemyPrefab; 
    private float maxObstaclePercentage = 0.10f; 
    private float maxEnemyPercentage = 0.05f; 
    private float maxDecorationPercentage = 0.05f;
    private float maxFillPercentage = 0.10f; 
    private float obstacle1x2SpawnChance = 0.20f;
    private float obstacle2x2SpawnChance = 0.10f;
    private float enemySpawnChance = 0.10f;
    private float decorationSpawnChance = 0.20f; 
    private int maxObstacleAttempts = 30; 

    public RoomFactory(GameObject tilePrefab, GameObject[] obstaclePrefabs1x2, GameObject[] obstaclePrefabs2x2, GameObject[] decorationPrefabs, GameObject enemyPrefab, int gridWidth, int gridHeight, int roomWidth, int roomHeight, int numberOfRooms)
    {
        Instance = this;
        this.tilePrefab = tilePrefab;
        this.obstaclePrefabs1x2 = obstaclePrefabs1x2;
        this.obstaclePrefabs2x2 = obstaclePrefabs2x2;
        this.decorationPrefabs = decorationPrefabs;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.roomWidth = roomWidth;
        this.roomHeight = roomHeight;
        this.numberOfRooms = numberOfRooms;
        this.enemyPrefab = enemyPrefab;
    }

    public List<Room> GenerateRooms()
    {
        rooms = new List<Room>();
        int attempts = 0;
        int maxAttempts = numberOfRooms * 10;
        int buffer = 3;
        Debug.Log(TileGenerator.allTiles.Count);    

        while (rooms.Count < numberOfRooms && attempts < maxAttempts)
        {
            attempts++;
            Vector2Int position = new Vector2Int(
                Random.Range(1, gridWidth - roomWidth - 1),
                Random.Range(1, gridHeight - roomHeight - 1)
            );
            Room room = new Room(position, roomWidth, roomHeight, tilePrefab);
            if (!AreEdgesOccupied(room, buffer))
            {
                rooms.Add(room);
                MarkTilesAsOccupied(room);
                room.MarkEdgeTilesAsCorridorConnectors();
            }
        }

        if (rooms.Count < numberOfRooms)
        {
            Debug.LogWarning("Could not generate the desired number of rooms. Generated " + rooms.Count + " rooms.");
        }
        Debug.Log(TileGenerator.allTiles.Count);
        Debug.Log("Number of rooms generated: " + rooms.Count);
        return rooms;
    }

    private bool AreEdgesOccupied(Room room, int buffer)
    {
        for (int x = room.Position.x - buffer; x <= room.Position.x + room.Tiles.GetLength(0) + buffer; x++)
        {
            for (int y = room.Position.y - buffer; y <= room.Position.y + room.Tiles.GetLength(1) + buffer; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (TileGenerator.allTiles.TryGetValue(position, out Tile tile))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void MarkTilesAsOccupied(Room room)
    {
        for (int x = 0; x < room.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < room.Tiles.GetLength(1); y++)
            {
                Tile tile = room.Tiles[x, y];
                if (tile != null)
                {
                    TileGenerator.Instance.AddTileToAllTiles(tile);
                }
            }
        }
    }

    public void ConnectRooms(List<Room> rooms)
    {
        HashSet<Room> connectedRooms = new HashSet<Room>();
        Room startRoom = rooms[0];
        connectedRooms.Add(startRoom);

        while (connectedRooms.Count < rooms.Count)
        {
            Room roomA = null;
            Room roomB = null;
            float minDistance = float.MaxValue;

            foreach (Room connectedRoom in connectedRooms)
            {
                foreach (Room room in rooms)
                {
                    if (connectedRooms.Contains(room)) continue;
                    float distance = Vector2Int.Distance(connectedRoom.GetCenter(), room.GetCenter());
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        roomA = connectedRoom;
                        roomB = room;
                    }
                }
            }

            if (roomA != null && roomB != null)
            {
                ConnectRoomCenters(roomA, roomB);
                connectedRooms.Add(roomB);
            }
        }
    }

    private void ConnectRoomCenters(Room roomA, Room roomB)
    {
        Vector2Int centerA = roomA.GetCenter();
        Vector2Int centerB = roomB.GetCenter();

        for (int x = Mathf.Min(centerA.x, centerB.x); x <= Mathf.Max(centerA.x, centerB.x); x++)
        {
            CreateCorridorTile(new Vector2Int(x, centerA.y));
        }

        for (int y = Mathf.Min(centerA.y, centerB.y); y <= Mathf.Max(centerA.y, centerB.y); y++)
        {
            CreateCorridorTile(new Vector2Int(centerB.x, y));
        }
    }

    private void CreateCorridorTile(Vector2Int position)
    {
        GameObject tileObject = GameObject.Instantiate(tilePrefab);
        tileObject.transform.position = new Vector3(position.x, 0, position.y);
        Tile tile = tileObject.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.LogError("Tile component is missing on the tile prefab at position: " + position);
            return;
        }
        tile.Position = position;
        tile.SetTileType("Normal");
        tile.IsCorridorTile = true;
        tile.GetComponent<Renderer>().material.color = Color.green;
        TileGenerator.Instance.AddTileToAllTiles(tile); 
    }

    public bool AreAllRoomsConnected()
    {
        HashSet<Room> connectedRooms = new HashSet<Room>();
        Queue<Room> queue = new Queue<Room>();
        queue.Enqueue(rooms[0]);
        connectedRooms.Add(rooms[0]);

        while (queue.Count > 0)
        {
            Room currentRoom = queue.Dequeue();
            foreach (Room room in rooms)
            {
                if (!connectedRooms.Contains(room))
                {
                    connectedRooms.Add(room);
                    queue.Enqueue(room);
                }
            }
        }

        foreach (Room room in rooms)
        {
            if (connectedRooms.Contains(room))
            {
                foreach (Tile tile in room.Tiles)
                {
                    // tile.GetComponent<Renderer>().material.color = Color.green;
                }
            }
            else
            {
                foreach (Tile tile in room.Tiles)
                {
                    // GameObject.Destroy(tile.gameObject);
                }
            }
        }

        Debug.Log("Number of connected rooms: " + connectedRooms.Count);
        return connectedRooms.Count == rooms.Count;
    }

    public void SpawnEnemiesInRooms(List<Room> currentRooms)
    {
        foreach (Room room in currentRooms)
        {
            int numberOfEnemies = Random.Range(1, 2); 
            for (int i = 0; i < numberOfEnemies; i++)
            {
                int x = Random.Range(0, room.Tiles.GetLength(0));
                int y = Random.Range(0, room.Tiles.GetLength(1));
                Tile tile = room.Tiles[x, y];
                if (tile != null && tile.IsWalkable && !tile.HasObstacleOrDecoration())
                {
                    Vector3 positionOffset = tile.transform.position; 
                    tile.PlaceEnemy(enemyPrefab, positionOffset);
                }
            }
        }
    }

    public void GenerateObstaclesDecorationsAndEnemies(List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
            int totalTiles = room.Tiles.GetLength(0) * room.Tiles.GetLength(1);
            int maxFillCount = Mathf.FloorToInt(totalTiles * maxFillPercentage);
            int fillCount = 0;
            int attempts = 0;

            List<Tile> shuffledTiles = new List<Tile>(room.Tiles.Cast<Tile>());
            shuffledTiles = shuffledTiles.OrderBy(t => Random.value).ToList(); 

            foreach (Tile tile in shuffledTiles)
            {
                if (!tile.IsWalkable || tile.HasObstacleOrDecoration() || tile.IsCorridorTile || IsNearEntrance(tile.Position, room) || usedPositions.Contains(tile.Position))
                    continue;

                if (fillCount >= maxFillCount || attempts >= maxObstacleAttempts)
                    break;

                float spawnRoll = Random.value;
                if (spawnRoll < obstacle1x2SpawnChance)
                {
                    if (CanPlace1x2Obstacle(tile.Position))
                    {
                        GameObject obstaclePrefab = obstaclePrefabs1x2[Random.Range(0, obstaclePrefabs1x2.Length)];
                        Vector3 obstaclePosition = tile.transform.position;
                        GameObject obstacle = Object.Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                        obstacle.transform.SetParent(tile.transform);
                        tile.SetTileType("Obstacle1x2");
                        tile.IsWalkable = false;

                        Tile adjacentTile = TileGenerator.Instance.GetTileAtPosition(tile.Position + Vector2Int.right);
                        if (adjacentTile != null) adjacentTile.IsWalkable = false;

                        usedPositions.Add(tile.Position);
                        usedPositions.Add(tile.Position + Vector2Int.right);
                        fillCount++;
                    }
                }
                else if (spawnRoll < obstacle1x2SpawnChance + obstacle2x2SpawnChance)
                {
                    if (CanPlace2x2Obstacle(tile.Position))
                    {
                        GameObject obstaclePrefab = obstaclePrefabs2x2[Random.Range(0, obstaclePrefabs2x2.Length)];
                        Vector3 obstaclePosition = tile.transform.position;
                        GameObject obstacle = Object.Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                        obstacle.transform.SetParent(tile.transform);
                        tile.SetTileType("Obstacle2x2");
                        tile.IsWalkable = false;

                        Tile adjacentTile = TileGenerator.Instance.GetTileAtPosition(tile.Position + Vector2Int.right);
                        if (adjacentTile != null) adjacentTile.IsWalkable = false;

                        usedPositions.Add(tile.Position);
                        usedPositions.Add(tile.Position + Vector2Int.right);
                        fillCount++;
                    }
                }
                else if (spawnRoll < obstacle1x2SpawnChance + obstacle2x2SpawnChance + enemySpawnChance)
                {
                    Vector3 positionOffset = tile.transform.position;
                    tile.PlaceEnemy(enemyPrefab, positionOffset);
                    tile.IsWalkable = false;
                    usedPositions.Add(tile.Position);
                    fillCount++;
                }
                else if (spawnRoll < obstacle1x2SpawnChance + obstacle2x2SpawnChance + enemySpawnChance + decorationSpawnChance)
                {
                    int decorationIndex = Random.Range(0, decorationPrefabs.Length);
                    GameObject decorationPrefab = decorationPrefabs[decorationIndex];
                    Vector3 positionOffset = tile.transform.position;
                    GameObject decoration = Object.Instantiate(decorationPrefab, positionOffset, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0), tile.transform);
                    tile.SetTileType("Decoration");
                    usedPositions.Add(tile.Position);
                    fillCount++;
                }

                attempts++;
            }
        }
    }

    private bool CanPlace1x2Obstacle(Vector2Int position)
    {
        Tile rightTile = TileGenerator.Instance.GetTileAtPosition(position + Vector2Int.right);
        return rightTile != null && rightTile.IsWalkable && !rightTile.HasObstacleOrDecoration();
    }

    private bool CanPlace2x2Obstacle(Vector2Int position)
    {
        Tile rightTile = TileGenerator.Instance.GetTileAtPosition(position + Vector2Int.right);
        return rightTile != null && rightTile.IsWalkable && !rightTile.HasObstacleOrDecoration();
    }

    private bool IsNearEntrance(Vector2Int position, Room room)
    {
        foreach (Tile tile in room.Tiles)
        {
            if (tile.IsCorridorTile && Vector2Int.Distance(position, tile.Position) < 1)
            {
                return true;
            }
        }
        return false;
    }

    public void GenerateSingleEnemyInRoom(Room room)
    {
        if (enemyPrefab != null)
        {
            Vector2Int enemyPosition = room.GetCenter();
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
            }
        }
    }
}