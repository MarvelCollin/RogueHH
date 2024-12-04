using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarSO", menuName = "ScriptableObjects/BarSO")]
public class BarSO : ScriptableObject
{
    public float currentValue;
    public float maxValue;

    public void ResetAndExtendMaxValue(float extension)
    {
        currentValue = 0;
        maxValue += extension;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
