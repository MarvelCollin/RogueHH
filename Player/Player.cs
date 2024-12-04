using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private PlayerState state;
    private Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();
    private bool isMoving = false;
    private Vector2Int currentDirection = Vector2Int.right;

    public List<SkillAbstract> skills = new List<SkillAbstract>();

    public Text damagePoints;

    [SerializeField] private float health;
    [SerializeField] private float attack;
    [SerializeField] private float defense;
    [SerializeField] private float criticalRate;
    [SerializeField] private float criticalDamage;
    [SerializeField] private float exp;
    [SerializeField] private int level = 1;

    public delegate void PlayerEvent(string eventType);
    public static event PlayerEvent OnPlayerEvent;

    public float Exp
    {
        get { return exp; }
        set { exp = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public float Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public float CriticalRate
    {
        get { return criticalRate; }
        set { criticalRate = value; }
    }

    public float CriticalDamage
    {
        get { return criticalDamage; }
        set { criticalDamage = value; }
    }

    public Animator Animator { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameFacade.Instance.Initialize();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        skills.Add(new StrengthBuffSkill());
        Animator = GetComponent<Animator>();

        // Register event listeners
        EventManager.StartListening("Stop", StopMovement);
        EventManager.StartListening("Attack", () => AttackEnemy(GetTargetEnemy()));
        EventManager.StartListening("PlayerTurn", OnPlayerTurn);
        EventManager.StartListening("EnemyTurn", OnEnemyTurn);
        EventManager.StartListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
    }

    void OnDestroy()
    {
        // Unregister event listeners
        EventManager.StopListening("Stop", StopMovement);
        EventManager.StopListening("Attack", () => AttackEnemy(GetTargetEnemy()));
        EventManager.StopListening("PlayerTurn", OnPlayerTurn);
        EventManager.StopListening("EnemyTurn", OnEnemyTurn);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
        EventManager.StopListening("EnemyTurnCompleted", OnEnemyTurnCompleted);
    }

    private void OnPlayerTurn()
    {
        // Handle player turn start
    }

    private void OnEnemyTurn()
    {
        // Handle enemy turn start
    }

    private void OnEnemyTurnCompleted()
    {
        // Handle enemy turn completion
        TileGenerator.Instance.setPlayerTurn();
    }

    public void AnimationToIdle()
    {
        Animator.SetTrigger("isIdle");
        Animator.ResetTrigger("isMoving");
        Animator.ResetTrigger("isAttacking");
    }

    public void AnimationToMoving()
    {
        Animator.ResetTrigger("isIdle");
        Animator.SetTrigger("isMoving");
        Animator.ResetTrigger("isAttacking");
    }

    public void AnimationToAttacking()
    {
        Animator.ResetTrigger("isIdle");
        Animator.ResetTrigger("isMoving");
        Animator.SetTrigger("isAttacking");
    }

    void Start()
    {
    }

    public void StartPlayer()
    {
        StartCoroutine(InitializePlayer());
    }

    private IEnumerator InitializePlayer()
    {
        while (!TileGenerator.Instance.IsInitialized)
        {
            yield return null;
        }

        SetState(new PlayerIdleState(this));
        SetIsometricCamera();
    }

    void Update()
    {
        if (TileGenerator.Instance.getPlayerTurn())
        {
            if (state != null)
            {
                state.Update();
            }
            UpdateIsometricCamera();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null && IsEnemyInAttackRange(enemy))
                    {
                        AttackEnemy(enemy);
                    }
                }
            }

            HandleSkillInput();
        }
    }

    private void HandleSkillInput()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                skills[i].UseSkillWithParticles();
            }
        }
    }

    private bool IsEnemyInAttackRange(Enemy enemy)
    {
        Vector3Int playerPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        Vector3Int enemyPosition = new Vector3Int((int)enemy.transform.position.x, (int)enemy.transform.position.y, (int)enemy.transform.position.z);

        return playerPosition + Vector3Int.left == enemyPosition ||
               playerPosition + Vector3Int.right == enemyPosition ||
               playerPosition + Vector3Int.forward == enemyPosition ||
               playerPosition + Vector3Int.back == enemyPosition;
    }

    public void SetState(PlayerState newState)
    {
        if (state != null)
        {
            state.OnExit();
        }
        state = newState;
        if (state != null)
        {
            state.OnEnter();
        }
    }

    public void TriggerEvent(string eventType)
    {
        if (state != null)
        {
            state.OnEvent(eventType);
        }
        OnPlayerEvent?.Invoke(eventType);

        // Trigger the event using the EventManager
        EventManager.TriggerEvent(eventType);
    }

    private Enemy GetTargetEnemy()
    {
        return FindObjectOfType<Enemy>();
    }

    public void ClearPath()
    {
        pathQueue.Clear();
    }

    public void SetInitialPosition(Vector2Int position)
    {
        transform.position = new Vector3(position.x, 0.5f, position.y);
    }

    public void SetPath(List<Vector2Int> path)
    {
        pathQueue = new Queue<Vector2Int>(path);
        SetState(new PlayerMovingState(this, pathQueue));
    }

    public void StopMovement()
    {
        if (state is PlayerMovingState)
        {
            ((PlayerMovingState)state).Stop();
        }
    }

    private void SetIsometricCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(transform.position.x - 10, transform.position.y + 10, transform.position.z - 10);
            mainCamera.transform.rotation = Quaternion.Euler(30, 45, 0);
            mainCamera.orthographic = true;
        }
    }

    private void UpdateIsometricCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(transform.position.x - 10, transform.position.y + 10, transform.position.z - 10);
            mainCamera.transform.rotation = Quaternion.Euler(30, 45, 0);
        }
    }

    public void AttackEnemy(Enemy enemy)
    {
        SetState(new PlayerAttackState(this, enemy));
    }

    public void TakeDamage(int damage, bool isCritical)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
        ShowDamagePoints(damage, isCritical);
    }

    private void Die()
    {
        // Implement death logic
    }

    public void GainExp(float expPoints)
    {
        Exp += expPoints;
        if (Exp >= PlayerUIManager.Instance.expBar.maxValue)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        Exp = 0;
        PlayerUIManager.Instance.expBar.ResetAndExtendMaxValue(Exp * 0.1f);
    }

    public void GainExpFromEnemy(Enemy enemy)
    {
        float expPoints = Random.Range(10, 20);
        GainExp(expPoints);
    }

    public void ShowDamagePoints(int damage, bool isCritical)
    {
        if (damagePoints != null)
        {
            damagePoints.text = damage.ToString();
            damagePoints.color = isCritical ? Color.red : Color.white;
            StartCoroutine(AnimateDamagePoints());
        }
    }

    private IEnumerator AnimateDamagePoints()
    {
        damagePoints.gameObject.SetActive(true);
        Vector3 originalScale = damagePoints.transform.localScale;

        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        damagePoints.transform.localScale = originalScale;
        yield return new WaitForSeconds(0.5f);
        damagePoints.gameObject.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public IEnumerator WaitForAnimationAndChangeState()
    {
        Animator animator = GetComponent<Animator>();
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
    }
}