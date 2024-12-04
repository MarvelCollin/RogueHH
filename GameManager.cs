using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject PlayerUI;
    // public GameObject PlayerInstance.;
    // public GameObject TileGeneratorInstance.;
    public GameObject MainMenu;
    public GameObject ShopMenu;

    public bool isStartGamePressed = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameFacade.Instance.Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // AttributeManager.Instance.ResetAll();
        
        // MainMenu.SetActive(false);
        // ShopMenu.SetActive(false);
        ShowMainMenu();
        // GoToGame();
        // GoToShopMenu();
    }

    void ResetAllMenus()
    {
        PlayerUI.SetActive(false);
        Player.Instance.gameObject.SetActive(false);
        TileGenerator.Instance.gameObject.SetActive(false);
        MainMenu.SetActive(false);
        ShopMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        ResetAllMenus();
        MainMenu.SetActive(true);
    }

    public void GoToShopMenu()
    {
        ResetAllMenus();
        ShopMenu.SetActive(true);
    }

    public void GoToGame(int floorLevel)
    {
        GameFacade.Instance.StoreAttributesToPlayer();
        ResetAllMenus();    
        PlayerUI.SetActive(true);
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.StartPlayer();
        TileGenerator.Instance.gameObject.SetActive(true);
        TileGenerator.Instance.floorLevel = floorLevel;
        TileGenerator.Instance.StartGame();
    }
}
