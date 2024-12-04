using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; } 
    
    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    public void OnNewGameButtonClick()
    {
        // MenuManager.Instance.OnNewGameButtonClick();
    }

    public void OnClickContinue()
    {
        GameManager.Instance.GoToShopMenu();
    }

    public void OnClickNewGame()
    {
        AttributeManager.Instance.ResetAll();
        GameManager.Instance.GoToShopMenu();
    }
}