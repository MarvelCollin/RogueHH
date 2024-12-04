using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2Int Position { get; private set; }
    public Tile[,] Tiles { get; private set; }
    private GameObject tilePrefab;

    public Room(Vector2Int position, int width, int height, GameObject tilePrefab)
    {
        Position = position;
        Tiles = new Tile[width, height];
        this.tilePrefab = tilePrefab;

        if (tilePrefab.GetComponent<Tile>() == null)
        {
            Debug.LogError("Tile prefab does not have a Tile component.");
            return;
        }

        GenerateTiles();
    }

    private void GenerateTiles()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab is not assigned.");
            return;
        }

        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                GameObject tileObject = GameObject.Instantiate(tilePrefab);
                tileObject.transform.position = new Vector3(Position.x + x, 0, Position.y + y);
                tileObject.transform.parent = null; 
                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    Debug.LogError("Tile component is missing on the tile prefab.");
                    return;
                }
                if (tileObject.GetComponent<Renderer>() == null)
                {
                    Debug.LogError("Tile prefab does not have a Renderer component.");
                    return;
                }
                tileObject.GetComponent<Renderer>().material.color = Color.black;
                tile.Position = new Vector2Int(Position.x + x, Position.y + y); 
                tile.Type = "Normal";
                tile.IsWalkable = true;
                Tiles[x, y] = tile;
                // Debug.Log("Tile generated at position: " + tile.Position + " with type: " + tile.Type);
            }
        }

        // Change the color of the center tile
        Vector2Int centerPosition = GetCenter();
        Tile centerTile = Tiles[centerPosition.x - Position.x, centerPosition.y - Position.y];
        centerTile.IsCenterTile = true;

        // Ensure corner tiles are walkable and have no obstacles
        MarkCornerTilesAsWalkable();
    }

    private void MarkCornerTilesAsWalkable()
    {
        Vector2Int[] cornerPositions = new Vector2Int[]
        {
            Position,
            Position + new Vector2Int(Tiles.GetLength(0) - 1, 0),
            Position + new Vector2Int(0, Tiles.GetLength(1) - 1),
            Position + new Vector2Int(Tiles.GetLength(0) - 1, Tiles.GetLength(1) - 1)
        };

        foreach (Vector2Int cornerPosition in cornerPositions)
        {
            Tile cornerTile = TileGenerator.Instance.GetTileAtPosition(cornerPosition);
            if (cornerTile != null)
            {
                cornerTile.IsWalkable = true;
                cornerTile.IsOccupied = false;
                cornerTile.GetComponent<Renderer>().material.color = Color.black;
            }
        }
    }

    public Vector2Int GetCenter()
    {
        return new Vector2Int(Position.x + Tiles.GetLength(0) / 2, Position.y + Tiles.GetLength(1) / 2);
    }

    public void MarkEdgeTilesAsCorridorConnectors()
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            Tiles[x, 0].IsCorridorTile = true; // Bawah
            Tiles[x, 0].IsWalkable = true; // Ensure walkable
            Tiles[x, Tiles.GetLength(1) - 1].IsCorridorTile = true; // Atas
            Tiles[x, Tiles.GetLength(1) - 1].IsWalkable = true; // Ensure walkable
        }
        for (int y = 0; y < Tiles.GetLength(1); y++)
        {
            Tiles[0, y].IsCorridorTile = true; // Kiri
            Tiles[0, y].IsWalkable = true; // Ensure walkable
            Tiles[Tiles.GetLength(0) - 1, y].IsCorridorTile = true; // Kanan
            Tiles[Tiles.GetLength(0) - 1, y].IsWalkable = true; // Ensure walkable
        }
    }
}