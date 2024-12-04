using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopMenu : MonoBehaviour
{
    public static ShopMenu Instance { get; private set; }     

    public Dropdown dropdown;
    
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
        List<string> options = new List<string> { "boss" };
        for (int i = 1; i <= 100; i++)
        {
            options.Add("floor " + i);
        }
        dropdown.AddOptions(options);
    }

    public void OnStartGameButtonClick()
    {
        GameManager.Instance.isStartGamePressed = true;
        GameManager.Instance.GoToGame(GetSelectedFloorLevel());
    }

    public void OnShopButtonClick()
    {
        // MenuManager.Instance.ShowShopMenu();
    }

    public void OnClickExit(){
        GameManager.Instance.ShowMainMenu();
    }

    public int GetSelectedFloorLevel()
    {
        Debug.Log("Selected " + dropdown.value);
        return dropdown.value;
    }
}