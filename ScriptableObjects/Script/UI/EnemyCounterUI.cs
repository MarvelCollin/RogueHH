using UnityEngine;
using UnityEngine.UI;

public class EnemyCounterUI : MonoBehaviour {
    public Text text;

    public IntegerSO enemyCountSO;

    void Update(){
        UpdateUI(enemyCountSO.value);
    }

    public void UpdateUI(int enemyCount){
        text.text = "Enemies: " + enemyCount;
    }    
}