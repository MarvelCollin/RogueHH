public abstract class EnemyState
{
    protected Enemy enemy;
    protected EnemyAttributes enemyAttributes;

    public EnemyState(Enemy enemy, EnemyAttributes enemyAttributes)
    {
        this.enemy = enemy;
        this.enemyAttributes = enemyAttributes;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}