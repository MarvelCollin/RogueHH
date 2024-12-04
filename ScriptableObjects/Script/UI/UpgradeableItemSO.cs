using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject/UpgradeableItem", menuName = "UpgradeableItem")]
public class UpgradeableItemSO : ScriptableObject
{
    public Sprite image;
    public string name;
    public string description;
    public int level;
    public int maxLevel;
    public float upgradeValue;
    public float baseValue;
    public float value;
    public int cost;

    public void UpdateAttributeUI(AbstractAttribute attribute)
    {
        attribute.levelText.text = $"{level}/{maxLevel}";
    }

    public void start()
    {
    }


    public int Cost
    {
        get { return cost; }
    }

    public float Value
    {
        get { return value; }
    }

    public Sprite Image
    {
        get { return image; }
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public int Level
    {
        get { return level; }
    }

    public int MaxLevel
    {
        get { return maxLevel; }
    }

    public float UpgradeValue
    {
        get { return upgradeValue; }
    }
    
}
