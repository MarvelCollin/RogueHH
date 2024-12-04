using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }

    public BarSO healthBar;
    public BarSO expBar;

    public IntegerSO enemyCounterSO;
    public IntegerSO floorLevelSO;

    void Update(){
        updateUI();
        
    }

    public void updateHealthBar(float currentValue){
        healthBar.currentValue = currentValue;
    }

    public void updateExpBar(float currentValue){
        expBar.currentValue = currentValue;
    }

    public void updateUI(){
        updateHealthBar(Player.Instance.Health);
        updateExpBar(Player.Instance.Exp);
    }

    public void setHealth(float value){
        healthBar.currentValue = value;
    }

    public void setExp(float value){
        expBar.currentValue = value;
    }

    public void setEnemyCounter(int value){
        enemyCounterSO.value = value;
    }

    public void setFloorLevel(int value){
        floorLevelSO.value = value;
    }
}
