using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealActiveSkill : SkillAbstract
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void UseSkill()
    {
        if (CanUseSkill())
        {
            IncreaseHealth();
            Debug.Log("Using Heal Skill");
            ResetCooldown();
        }
    }

    public void IncreaseHealth(){
        Player.Instance.Health += 100;
    }
}
