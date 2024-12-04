using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance { get; private set; }

    public AttackAttribute attackAttribute;
    public CriticalChanceAttribute criticalChanceAttribute;
    public CriticalDamageAttribute criticalDamageAttribute;
    public DefenseAttribute defenseAttribute;   
    public HealthAttribute healthAttribute;
    public ViewAttribute viewAttribute;

    public float gold = 1000;

    public AbstractAttribute selectedAttribute;


    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            GameFacade.Instance.Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("AttributeManager Awake");
    }

    public void HandleOnClickAttribute(AbstractAttribute attribute)
    {
        selectedAttribute = attribute;
        attribute.upgradeableItemSO.UpdateAttributeUI(viewAttribute);
        viewAttribute.changeView(attribute.upgradeableItemSO);
    }

    public float getAttackValue(){
        return attackAttribute.upgradeableItemSO.value;
    }

    public float getCriticalChanceValue(){
        return criticalChanceAttribute.upgradeableItemSO.value;
    }

    public float getCriticalDamageValue(){
        return criticalDamageAttribute.upgradeableItemSO.value;
    }

    public float getDefenseValue(){
        return defenseAttribute.upgradeableItemSO.value;
    }

    public float getHealthValue(){
        return healthAttribute.upgradeableItemSO.value;
    }

    public void ResetAll(){
        attackAttribute.ResetData(5,2);
        criticalChanceAttribute.ResetData(5,2);
        criticalDamageAttribute.ResetData(150, 5);
        defenseAttribute.ResetData(5,5);
        healthAttribute.ResetData(20, 10);
        // viewAttribute.ResetData(0,0);
    }

    public void StoreToPlayer(){
        Player.Instance.Attack = getAttackValue();
        Player.Instance.CriticalRate = getCriticalChanceValue();
        Player.Instance.CriticalDamage = getCriticalDamageValue();
        Player.Instance.Defense = getDefenseValue();
        Player.Instance.Health = getHealthValue();
    }

    public void IncreaseOtherAttributesCost(AbstractAttribute selectedAttribute, float costIncrease)
    {
        List<AbstractAttribute> attributes = new List<AbstractAttribute>
        {
            attackAttribute,
            criticalChanceAttribute,
            criticalDamageAttribute,
            defenseAttribute,
            healthAttribute
        };

        foreach (var attribute in attributes)
        {
            if (attribute != selectedAttribute)
            {
                attribute.upgradeableItemSO.cost += (int)costIncrease;
            }
        }
    }
}
