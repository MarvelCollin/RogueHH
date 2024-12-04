using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, EnemyAttributes enemyAttributes) : base(enemy, enemyAttributes) { }

    public override void Enter()
    {
        // enemy.AnimationToAttacking();

        enemy.SetStatus("TOT");
        PerformAttack();
        enemy.SetState(new EnemyAggroState(enemy, enemyAttributes));
    }

    public override void Update()
    {
        // Attack logic here
    }

    public override void Exit()
    {
        enemy.Animator.ResetTrigger("isAttacking");
    }

    private void PerformAttack()
    {
        float baseDamage = enemy.enemyAttributes.Attack;
        bool isCritical = Random.value <= enemy.enemyAttributes.CriticalRate;
        float criticalMultiplier = isCritical ? enemy.enemyAttributes.CriticalDamage : 1f;
        float damage = baseDamage * criticalMultiplier * Random.Range(0.75f, 1.25f);
        damage -= Player.Instance.Defense; 
        damage = Mathf.Max(damage, 0);
        if (isCritical)
        {
            Player.Instance.TakeDamage((int)damage, true);
        }
        else
        {
            Player.Instance.TakeDamage((int)damage, false);
        }
        TileGenerator.Instance.EnemyMoveCompleted();
        enemy.CompleteTurn();
    }
}