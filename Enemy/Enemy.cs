using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Enemy : MonoBehaviour
{

    public enum EnemyType
    {
        Common,
        Medium,
        Elite
    }

    private EnemyState currentState;
    public Vector2 roomSize;
    public float alertRange = 5f;
    public Vector2Int currentDirection = Vector2Int.right;
    public bool isMoving = false;
    private bool playerMoved = false;
    public Animator Animator { get; private set; }
    public Text nameText;
    public Text statusText;
    public Scrollbar healthBar;
    private float maxHealth = 100f;
    private bool canClick = true;
    private float clickDelay = 0.5f;
    public Text damagePoints;

    public EnemyAttributes enemyAttributes;
    public EnemyType enemyType;

    public bool HasCompletedTurn { get; private set; }

    void Start()
    {
        nameText.text = GameFacade.Instance.GetRandomEnemyName();
        Color enemyColor = GameFacade.Instance.GetRandomEnemyType();
        nameText.color = enemyColor;

        int floorLevel = GameFacade.Instance.GetFloorLevel();
        enemyAttributes = GetEnemyAttributes(enemyColor, floorLevel);
        enemyType = GetEnemyType(enemyColor);

        alertRange = enemyAttributes.AttackRange;
        maxHealth = enemyAttributes.Defense * 10;
        enemyAttributes.Health = maxHealth;

        UpdateHealthBar();
        SetState(new EnemyIdleState(this, enemyAttributes));
    }

    void Awake()
    {
        Animator = GetComponent<Animator>();

        // Register event listeners
        EventManager.StartListening("PlayerTurn", OnPlayerTurn);
        EventManager.StartListening("EnemyTurn", OnEnemyTurn);
    }

    void OnDestroy()
    {
        // Unregister event listeners
        EventManager.StopListening("PlayerTurn", OnPlayerTurn);
        EventManager.StopListening("EnemyTurn", OnEnemyTurn);
    }

    private void OnPlayerTurn()
    {
        // Handle player turn start
    }

    private void OnEnemyTurn()
    {
        // Handle enemy turn start
    }

    void Update()
    {
        currentState?.Update();

        if (playerMoved)    
        {
            playerMoved = false;
            currentState?.Update();
        }
    }

    public bool HasPlayerClicked()
    {
        return Input.GetMouseButtonDown(0);
    }

    public void SetState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        if (currentState is EnemyAttackState)
        {
            TileGenerator.Instance.RegisterEnemyMove();
        }
        else
        {
            CompleteTurn();
        }
    }

    public bool IsInDistance()
    {
        Player player = Player.Instance;

        float distanceToPlayer = Vector3.Distance(transform.position, player.GetPosition());

        if (!HasLineOfSightToPlayer()) return false;

        Vector3 directionToPlayer = (player.GetPosition() - transform.position).normalized;
        Vector3 currentPosition = transform.position;
        while (Vector3.Distance(currentPosition, player.GetPosition()) > 1f)
        {
            currentPosition += directionToPlayer;
            Tile tile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)currentPosition.x, (int)currentPosition.z));

            // if (tile == null || tile.Type.StartsWith("Obstacle") || tile.Type == "Decoration" || tile.Type == "Empty")
            // {
            //     return false;
            // }
        }
        return true;
    }

    public bool HasLineOfSightToPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.Log("Player not found");
            return false;
        }

        Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            Vector3 currentPosition = transform.position;
            while (Vector3.Distance(currentPosition, playerObject.transform.position) > 1f)
            {
                currentPosition += directionToPlayer;
                Tile tile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)currentPosition.x, (int)currentPosition.z));

                if (tile == null || tile.Type.StartsWith("Obstacle"))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public bool IsDead()
    {
        return false;
    }

    public void OnPlayerMove()
    {
        playerMoved = true;
        if (IsInDistance() && HasLineOfSightToPlayer())
        {
            StartCoroutine(ChasePlayerTurn());
        }
    }

    private IEnumerator ChasePlayerTurn()
    {
        yield return new WaitForSeconds(0.5f);
        currentState?.Update();
    }

    public void ValidateAndStartChase(Vector2Int targetPosition)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        Tile startTile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)transform.position.x, (int)transform.position.z));
        Tile targetTile = TileGenerator.Instance.GetTileAtPosition(targetPosition);

        if (startTile != null && targetTile != null && targetTile.IsWalkable && !IsTileOccupied(targetTile))
        {
            StartCoroutine(ChasePlayerTurn());
        }
    }

    private bool IsTileOccupied(Tile tile)
    {
        Collider[] colliders = Physics.OverlapBox(tile.transform.position, tile.transform.localScale / 2);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsInAggroState()
    {
        return currentState is EnemyAggroState;
    }

    public bool IsInAttackState()
    {
        return currentState is EnemyAttackState;
    }

    public bool IsPlayerInAttackRange()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return false;

        Vector3Int enemyPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        Vector3Int playerPosition = new Vector3Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.y, (int)playerObject.transform.position.z);

        // Debug.Log("Player" + playerPosition);
        // Debug.Log(enemyPosition + new Vector3Int(-1, 0, 0));
        // Debug.Log(enemyPosition + new Vector3Int(1, 0, 0));
        // Debug.Log(enemyPosition + new Vector3Int(0, 0, 1));
        // Debug.Log(enemyPosition + new Vector3Int(0, 0, -1));


        if (enemyPosition + new Vector3Int(-1, 0, 0) == playerPosition)
        {
            return true;
        }
        if (enemyPosition + new Vector3Int(1, 0, 0) == playerPosition)
        {
            return true;
        }
        if (enemyPosition + new Vector3Int(0, 0, 1) == playerPosition)
        {
            return true;
        }
        if (enemyPosition + new Vector3Int(0, 0, -1) == playerPosition)
        {
            return true;
        }

        return false;
    }

    public void TakeDamage(int damage, bool isCritical = false)
    {
        damage -= enemyAttributes.Defense; 
        damage = Mathf.Max(damage, 0);
        enemyAttributes.Health -= damage;
        if (enemyAttributes.Health < 0) enemyAttributes.Health = 0;
        UpdateHealthBar();
        ShowDamagePoints(damage, isCritical);

        Debug.Log("Enemy Health: " + enemyAttributes.Health);
        if (enemyAttributes.Health <= 0)
        {
            Debug.Log("DAH MATI Enemy Health: " + enemyAttributes.Health);
            // SetState(new EnemyDieState(this, enemyAttributes));
            Enemy currentEnemy = this;
            Tile currentTile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)currentEnemy.transform.position.x, (int)currentEnemy.transform.position.z));
            if (currentTile != null)
            {
                currentTile.SetWalkable(true);
                currentTile.IsOccupied = false;
            }
            GameObject.Destroy(this.gameObject);
            Player.Instance.GainExpFromEnemy(currentEnemy);
            Debug.Log("Current enemy: " + currentEnemy);
        }
    }

    private void ShowDamagePoints(int damage, bool isCritical)
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

        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        damagePoints.gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.size = enemyAttributes.Health / maxHealth;
        }
    }

    void OnMouseDown()
    {
        if (TileGenerator.Instance.getPlayerTurn() && IsPlayerInAttackRange() && canClick)    
        {
            canClick = false;
            Debug.Log("Enemy clicked by player.");
            Player.Instance.AttackEnemy(this);
            StartCoroutine(ResetClick());
        }
    }

    private IEnumerator ResetClick()
    {
        yield return new WaitForSeconds(clickDelay);
        canClick = true;
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

    public void AnimationToAlerting()
    {
        Animator.ResetTrigger("isIdle");
        Animator.ResetTrigger("isMoving");
        Animator.ResetTrigger("isAttacking");
        Animator.SetTrigger("isAlerting");
    }

    public void SetStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }

    public EnemyAttributes GetEnemyAttributes(Color enemyColor, int floorLevel)
    {
        int baseAttackRange = 1;
        int baseDefense = 5;
        float baseAttack = 10f;
        float baseCriticalRate = 0.1f;
        float baseCriticalDamage = 1.5f;

        if (enemyColor == Color.white)
        {
            baseAttackRange = 1;
            baseDefense = 5;
            baseAttack = 10f;
            baseCriticalRate = 0.1f;
            baseCriticalDamage = 1.5f;
        }
        else if (enemyColor == Color.yellow)
        {
            baseAttackRange = (int)(1.5 * baseAttackRange);
            baseDefense = (int)(1.5 * baseDefense);
            baseAttack = 15f;
            baseCriticalRate = 0.15f;
            baseCriticalDamage = 1.75f;
        }
        else if (enemyColor == Color.red)
        {
            baseAttackRange = 2 * baseAttackRange;
            baseDefense = 2 * baseDefense;
            baseAttack = 20f;
            baseCriticalRate = 0.2f;
            baseCriticalDamage = 2f;
        }

        int attackRange = baseAttackRange + (floorLevel / 10);
        int defense = baseDefense + (floorLevel * 2);
        float attack = baseAttack + (floorLevel * 1.5f);
        float criticalRate = baseCriticalRate + (floorLevel * 0.01f);
        float criticalDamage = baseCriticalDamage + (floorLevel * 0.1f);

        return new EnemyAttributes
        {
            AttackRange = attackRange,
            Defense = defense,
            Attack = attack,
            CriticalRate = criticalRate,
            CriticalDamage = criticalDamage
        };
    }

    public EnemyType GetEnemyType(Color enemyColor)
    {
        if (enemyColor == Color.white)
        {
            return EnemyType.Common;
        }
        else if (enemyColor == Color.yellow)
        {
            return EnemyType.Medium;
        }
        else if (enemyColor == Color.red)
        {
            return EnemyType.Elite;
        }
        return EnemyType.Common;
    }

    public bool IsPlayerInRadius(float radius)
    {
        if(!Player.Instance){
            Debug.Log("PLayernya ga ada king");
            return false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.GetPosition());
        return distanceToPlayer <= radius;
    }

    public void TakeTurn()
    {
        HasCompletedTurn = false;
        if (IsPlayerInAttackRange())
        {
            SetState(new EnemyAttackState(this, enemyAttributes));
        }
        else
        {
            // Move towards player or perform other actions
            // ...
            HasCompletedTurn = true;
        }
        EventManager.TriggerEvent("EnemyTurnCompleted");
    }

    public void CompleteTurn()
    {
        HasCompletedTurn = true;
    }
}