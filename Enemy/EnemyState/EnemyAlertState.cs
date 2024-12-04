using UnityEngine;

public class EnemyAlertState : EnemyState
{
    public EnemyAlertState(Enemy enemy, EnemyAttributes enemyAttributes) : base(enemy, enemyAttributes) { }

    public override void Enter()
    {
        enemy.SetStatus("??");
    }

    public override void Update()
    {
        if (enemy.HasLineOfSightToPlayer())
        {
            enemy.SetState(new EnemyAggroState(enemy, enemyAttributes));
        }
        else if (!enemy.IsPlayerInRadius(8f))
        {
            enemy.SetState(new EnemyIdleState(enemy, enemyAttributes));
        }
    }

    public override void Exit()
    {
        enemy.Animator.ResetTrigger("isAlerting");
    }
}