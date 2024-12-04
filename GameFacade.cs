
using UnityEngine;

public class GameFacade
{
    public static GameFacade Instance { get; private set; } = new GameFacade();

    private GameFacade() { }

    public void Initialize()
    {
        
    }

    public void StartGame(int floorLevel)
    {
        if (TileGenerator.Instance.tilePrefab == null)
        {
            Debug.LogError("TilePrefab is not assigned in the TileGenerator.");
            return;
        }

        if (floorLevel == 0)
        {
            TileGenerator.Instance.GenerateSingleRoomWithObstacles();
        }
        else
        {
            TileGenerator.Instance.StartCoroutine(TileGenerator.Instance.GenerateAndConnectRooms());
            TileGenerator.Instance.StartCoroutine(TileGenerator.Instance.MoveColoredTilesAndPlayer());
        }

        TileGenerator.Instance.OptimizedEnding();
        TileGenerator.Instance.SetAllTilesToDark();
        TileGenerator.Instance.SetupPlayerLight();
        TileGenerator.Instance.HighlightCurrentPlayerTile();
        TileGenerator.Instance.SpawnPlayerInRandomRoom();
        PlayerUIManager.Instance.floorLevelSO.value = floorLevel;
        PlayerUIManager.Instance.setEnemyCounter(TileGenerator.Instance.getEnemyCounter());
    }

    public string GetRandomEnemyName()
    {
        return TileGenerator.Instance.GetRandomEnemyName();
    }

    public Color GetRandomEnemyType()
    {
        return TileGenerator.Instance.getRandomEnemyType();
    }

    public int GetFloorLevel()
    {
        return TileGenerator.Instance.floorLevel;
    }

    public void StoreAttributesToPlayer()
    {
        AttributeManager.Instance.StoreToPlayer();
    }
}