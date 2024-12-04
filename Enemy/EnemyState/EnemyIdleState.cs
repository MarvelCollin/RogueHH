using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, EnemyAttributes enemyAttributes) : base(enemy, enemyAttributes) { }

    public override void Enter()
    {
        enemy.AnimationToIdle();
        enemy.SetStatus("Zzz");
    }

    public override void Update()
    {
        if (enemy.IsPlayerInRadius(8f))
        {
            enemy.SetState(new EnemyAlertState(enemy, enemyAttributes));
        } 
    }

    public override void Exit()
    {
        enemy.Animator.ResetTrigger("isIdle");
    }
}