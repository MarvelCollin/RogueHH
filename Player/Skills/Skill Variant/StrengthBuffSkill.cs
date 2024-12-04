using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrengthBuffSkill : SkillAbstract
{
    public float attackBuffValue = 30;
    public float defenseBuffValue = 10;

    void Start()
    {
        skillType = SkillType.Buff;
        InitializeSkill();
    }

    void Update()
    {
        UpdateCooldown();
    }

    public override void UseSkill()
    {
        if (CanUseSkill())
        {
            Debug.Log("Using Strength Buff Skill");
            ApplyBuff(Player.Instance);
            ResetCooldown();
        }
    }

    public void ApplyBuff(Player player)
    {
        player.Attack += player.Attack * 0.5f;
        player.Defense += player.Defense * 0.5f;
        player.CriticalRate += player.CriticalRate * 0.5f;
        player.CriticalDamage += player.CriticalDamage * 0.5f;
        Debug.Log("Buff applied: Attack " + player.Attack + ", Defense " + player.Defense + ", Critical Rate " + player.CriticalRate + ", Critical Damage " + player.CriticalDamage + ", Health " + player.Health);
        player.StartCoroutine(RemoveBuffAfterDuration(player, 0.5f));
    }

    private IEnumerator RemoveBuffAfterDuration(Player player, float buffMultiplier)
    {
        yield return new WaitForSeconds(activeTime);
        player.Attack /= 1 + buffMultiplier;
        player.Defense /= 1 + buffMultiplier;
        player.CriticalRate /= 1 + buffMultiplier;
        player.CriticalDamage /= 1 + buffMultiplier;
        Debug.Log("Buff removed: Attack " + player.Attack + ", Defense " + player.Defense + ", Critical Rate " + player.CriticalRate + ", Critical Damage " + player.CriticalDamage + ", Health " + player.Health);
    }
}
