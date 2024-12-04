using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractAttribute : MonoBehaviour
{
    public UpgradeableItemSO upgradeableItemSO;

    public Text levelText;
    public Image image;

    public Button button;


    public abstract void Update();

    public void Upgrade()
    {
        if (upgradeableItemSO.level < upgradeableItemSO.maxLevel)
        {
            upgradeableItemSO.level++;
            upgradeableItemSO.value = CalculateValue();
            upgradeableItemSO.cost += 50;

            AttributeManager.Instance.IncreaseOtherAttributesCost(this, 10);
        }
    }

    private float CalculateValue()
    {
        return upgradeableItemSO.baseValue + (upgradeableItemSO.level * upgradeableItemSO.upgradeValue);
    }

    public void UpdateUI()
    {
        levelText.text = $"{upgradeableItemSO.level}/45";
    }

    public void PressedButton(){
        AttributeManager.Instance.HandleOnClickAttribute(this);
    }

    public void ResetData(float baseValue, float upgradeValue){
        upgradeableItemSO.level = 1;
        upgradeableItemSO.value = baseValue;
        upgradeableItemSO.cost = 10;
        upgradeableItemSO.baseValue = baseValue;
        upgradeableItemSO.upgradeValue = upgradeValue;
        upgradeableItemSO.maxLevel = 45;
    }
}
