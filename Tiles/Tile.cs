using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    [SerializeField] public string Type { get; set; }
    [SerializeField] public bool IsWalkable { get; set; }
    public GameObject[] Obstacles1x2;
    public GameObject[] Obstacles2x2;
    public GameObject[] Decorations;
    [SerializeField]  public bool IsCorridorTile { get; set; }
    [SerializeField] public bool IsCenterTile { get; set; }
    [SerializeField] public bool IsOccupied { get; set; }
    private Enemy enemy;
    private Color originalColor;

    public bool HasEnemy()
    {
        return enemy != null;
    }

    public Enemy GetEnemy()
    {
        return enemy;
    }

    public bool HasObstacleOrDecoration()
    {
        return transform.childCount > 0;
    }

    public void SetTileType(string type)
    {
        if (HasObstacleOrDecoration() || IsCorridorTile || IsCenterTile) return;

        Type = type;
        if (type == "Obstacle1x2")
        {
            if (Obstacles1x2.Length > 0)
            {
                int index = Random.Range(0, Obstacles1x2.Length);
                Vector3 positionOffset = transform.position;
                Instantiate(Obstacles1x2[index], positionOffset, Quaternion.identity, transform);
                IsWalkable = false;
                GetComponent<Renderer>().material.color = Color.red;

                Tile adjacentTile = TileGenerator.Instance.GetTileAtPosition(Position + Vector2Int.right);
                if (adjacentTile != null)
                {
                    adjacentTile.IsWalkable = false;
                    adjacentTile.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else if (type == "Obstacle2x2")
        {
            if (Obstacles2x2.Length > 0)
            {
                int index = Random.Range(0, Obstacles2x2.Length);
                Vector3 positionOffset = transform.position;
                GameObject obstacle = Instantiate(Obstacles2x2[index], positionOffset, Quaternion.identity, transform);
                IsWalkable = false;
                IsOccupied = true;
                GetComponent<Renderer>().material.color = Color.red;

                // Mark the adjacent tile as part of the obstacle
                Tile adjacentTile = TileGenerator.Instance.GetTileAtPosition(Position + Vector2Int.right);
                if (adjacentTile != null)
                {
                    adjacentTile.IsWalkable = false;
                    adjacentTile.IsOccupied = true;
                    adjacentTile.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else if (type == "Decoration")
        {
            if (Decorations.Length > 0)
            {
                int index = Random.Range(0, Decorations.Length);
                Vector3 positionOffset = transform.position;
                Instantiate(Decorations[index], positionOffset, Quaternion.identity, transform);
            }
        }
        else if (type == "Normal")
        {
            IsWalkable = true;
        }
        else
        {
            IsWalkable = false;
            // Destroy(gameObject); 
        }
        // Ensure edge tiles are walkable
        if (IsCorridorTile || IsCornerTile())
        {
            IsWalkable = true;
        }
    }

    public bool IsCornerTile()
    {
        // Check if the tile is at the corner of the room
        Room room = TileGenerator.Instance.GetRoomContainingTile(this);
        if (room != null)
        {
            Vector2Int roomPosition = room.Position;
            int roomWidth = room.Tiles.GetLength(0);
            int roomHeight = room.Tiles.GetLength(1);

            return (Position == roomPosition ||
                    Position == roomPosition + new Vector2Int(roomWidth - 1, 0) ||
                    Position == roomPosition + new Vector2Int(0, roomHeight - 1) ||
                    Position == roomPosition + new Vector2Int(roomWidth - 1, roomHeight - 1));
        }
        return false;
    }

    public Tile getTileFromPosition(Vector2Int position)
    {
        return this;
    }

    public void PlaceEnemy(GameObject enemyPrefab, Vector3? positionOffset)
    {
        if (enemyPrefab != null)
        {
            Vector3 spawnPosition = positionOffset ?? transform.position;
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.roomSize = new Vector2(1, 1);
                enemyObject.transform.position = spawnPosition;
                enemyObject.transform.SetParent(transform);
                Debug.Log("Enemy placed at position: " + enemyObject.transform.position);
                SetWalkable(false);
            }
        }
    }

    public void SetWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
        GetComponent<Renderer>().material.color = isWalkable ? GetComponent<Renderer>().sharedMaterial.color : Color.red;
        IsOccupied = !isWalkable;
    }

    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.black;
        // if (Type == "Obstacle")
        // {
        //     SetTileType("Obstacle");
        // }
        // else
        // {
        //     SetTileType("Normal");
        // }
    }

    void Update()
    {

    }

    void OnMouseEnter()
    {
        TileGenerator.Instance.ClearHoverPathHighlight();
        // Debug.Log("Type: " + Type + " IsWalkable: " + IsWalkable);
        if (TileGenerator.Instance.HasPermanentPath()) return;
        if (!TileGenerator.Instance.IsHoverPathTile(this))
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        TileGenerator.Instance.HighlightHoverPathToTile(this);
    }

    void OnMouseExit()
    {
        if (TileGenerator.Instance != null)
        {
            TileGenerator.Instance.ClearHoverPathHighlight();
        }
    }

    void OnMouseDown()
    {
        TileGenerator.Instance.StopPlayerMovement();
        TileGenerator.Instance.ClearClickPathHighlight();
        TileGenerator.Instance.FindPathToTile(this);
        TileGenerator.Instance.HighlightClickPathToTile(this);
        TileGenerator.Instance.SetClickedPath();
    }

    public void ResetColor()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }
}