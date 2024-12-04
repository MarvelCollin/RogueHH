using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public GameObject mainMenu;
    public GameObject shopMenu;
    public Camera mainMenuCamera;
    public Camera playerCamera;

    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    private void Start()
    {
        // ShowMainMenu(); 
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        shopMenu.SetActive(false);
        mainMenuCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
    }

    public void ShowShopMenu()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(true);
        mainMenuCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        // Remove display switching
        shopMenu.GetComponent<Canvas>().targetDisplay = 0; 
        mainMenuCamera.targetDisplay = 0; 
    }

    public void ShowPlayerView()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(false);
        mainMenuCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        playerCamera.targetDisplay = 0; 
    }

    public void OnNewGameButtonClick()
    {
        // ShowShopMenu();
    }

    public void OnStartGameButtonClick()
    {
        // ShowPlayerView();
    }
}