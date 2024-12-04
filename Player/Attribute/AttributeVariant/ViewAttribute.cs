using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewAttribute : AbstractAttribute
{
    public Button buyButton;
    public Text descriptionText;
    public Text nameText;
    public Text costText;
    public Text valueText;
    public Text upgradeValueText;

    public void Start()
    {
        changeView(AttributeManager.Instance.attackAttribute.upgradeableItemSO);
    }

    public override void Update()
    {
        updateViewUI();
    }

    public void changeView(UpgradeableItemSO upgradeableItemSO)
    {
        this.upgradeableItemSO = upgradeableItemSO;
        updateViewUI();
    }

    public void updateViewUI(){
        nameText.text = upgradeableItemSO.name;
        descriptionText.text = upgradeableItemSO.description;
        costText.text = upgradeableItemSO.cost.ToString();
        valueText.text = $"Value : {upgradeableItemSO.value}";
        upgradeValueText.text = $"Upgrade Value : {upgradeableItemSO.upgradeValue}";
        image.sprite = upgradeableItemSO.image;
        levelText.text = $"{upgradeableItemSO.level}/{upgradeableItemSO.maxLevel}";
    }

    public void BuyAttribute(){
        // if (upgradeableItemSO.cost <= AttributeManager.Instance.gold)
        // {
            // AttributeManager.Instance.gold -= upgradeableItemSO.cost;
            AttributeManager.Instance.selectedAttribute.Upgrade();
        // }
    }
    
}
