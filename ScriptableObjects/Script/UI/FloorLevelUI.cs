using UnityEngine;
using UnityEngine.UI;

public class FloorLevelUI : MonoBehaviour {
    public Text text;

    public IntegerSO floorLevelSO;

    void Update(){
        UpdateUI();
    }

    public void UpdateUI(){
        floorLevelSO.value = TileGenerator.Instance.floorLevel;

        if(floorLevelSO.value == 0){
            text.text = "Boss Level";
            return;
        }
        text.text = "Floor " + floorLevelSO.value;
    }
}