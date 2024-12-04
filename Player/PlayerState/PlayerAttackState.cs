using UnityEngine;
using System.Collections;

public class PlayerAttackState : PlayerState
{
    private Player player;
    private Enemy targetEnemy;

    public PlayerAttackState(Player player, Enemy targetEnemy)
    {
        this.player = player;
        this.targetEnemy = targetEnemy;
    }

    public void Update()
    {

    }

    private void PerformAttack()
    {
        player.AnimationToAttacking();
        player.StartCoroutine(player.WaitForAnimationAndChangeState());
        float baseDamage = Player.Instance.Attack;
        Debug.Log("Base Damagenya adalah " + baseDamage);
        Debug.Log("Critical Rate Player adalah " + Player.Instance.CriticalRate);
        bool isCritical = Random.value <= Player.Instance.CriticalRate;
        float criticalMultiplier = isCritical ? (1 + Player.Instance.CriticalDamage / 100) : 1f;
        float damage = baseDamage * criticalMultiplier * Random.Range(1f, 1.5f);
        targetEnemy.TakeDamage((int)damage, isCritical);

    }

    public void OnEnter()
    {
        Debug.Log("Masuk ke attack state");
        PerformAttack();
        TileGenerator.Instance.setEnemyTurn();  
        // player.SetState(new PlayerIdleState(player));
    }

    public void OnExit()
    {

    }

    private int CalculateDamage()
    {
        return 1100;
    }

    public void OnEvent(string eventType)
    {
        // Handle events if needed
    }
}