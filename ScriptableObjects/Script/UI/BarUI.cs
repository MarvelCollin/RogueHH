using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public BarSO barSO;
    public Image imageBar;

    public Text textBar;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(barSO != null){
            imageBar.fillAmount = barSO.currentValue / barSO.maxValue;
            textBar.text = $"{barSO.currentValue} / {barSO.maxValue}";
        }   
    }
}
