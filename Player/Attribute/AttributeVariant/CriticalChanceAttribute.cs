using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalChanceAttribute : AbstractAttribute
{
    public override void Update()
    {
        UpdateUI();
    }
}
