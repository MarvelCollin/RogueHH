using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAggroState : EnemyState
{
    public EnemyAggroState(Enemy enemy, EnemyAttributes enemyAttributes) : base(enemy, enemyAttributes) { }

    public override void Enter()
    {
        Debug.Log("Masuk Aggro State mas");
        enemy.SetStatus("!!");
    }

    public override void Update()
    {

        if (TileGenerator.Instance.getEnemyTurn())
        {
            // Debug.Log(TileGenerator.Instance.getEnemyToMove());

            if (enemy.IsPlayerInAttackRange())
            {
                enemy.SetState(new EnemyAttackState(enemy, enemyAttributes));
                // enemy.AnimationToIdle();

                TileGenerator.Instance.setPlayerTurn();
            }
            else
            {
                Debug.Log("lagi chase player - EnemyAggroState");

                ChasePlayer();
            }
        } else {
            enemy.AnimationToIdle();
        }
    }

    public override void Exit()
    {
        
    }

    public void ChasePlayer()
    {
        if (TileGenerator.Instance.getPlayerTurn()) return ;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");


        Tile startTile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)enemy.transform.position.x, (int)enemy.transform.position.z));
        Tile targetTile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)playerObject.transform.position.x, (int)playerObject.transform.position.z));
        List<Tile> path = TileGenerator.Instance.FindPath(startTile, targetTile, true);
        enemy.AnimationToMoving();

        if (path != null && path.Count > 1)
        {
            Vector2Int nextPosition = path[1].Position;
            Tile nextTile = TileGenerator.Instance.GetTileAtPosition(nextPosition);

            if (nextTile != null && nextTile.IsWalkable && !nextTile.IsOccupied)
            {
                nextTile.IsOccupied = true;
                Vector2Int direction = nextPosition - new Vector2Int((int)enemy.transform.position.x, (int)enemy.transform.position.z);

                if (direction != enemy.currentDirection)
                {
                    enemy.StartCoroutine(FaceDirection(direction));
                    enemy.currentDirection = direction;
                }

                enemy.StartCoroutine(MoveToPosition(nextPosition));
            }
            else
            {
                Debug.Log("Next tile is occupied or not walkable, enemy stays");
            }
        }
        else
        {
            Debug.Log("No path found or path is too short");
            MoveToAlternativePosition(startTile, targetTile);
        }
        TileGenerator.Instance.EnemyMoveCompleted();
    }

    private void MoveToAlternativePosition(Tile startTile, Tile targetTile)
    {
        List<Tile> alternativePath = TileGenerator.Instance.FindPath(startTile, targetTile, true);

        if (alternativePath != null && alternativePath.Count > 1)
        {
            Vector2Int nextPosition = alternativePath[1].Position;
            Tile nextTile = TileGenerator.Instance.GetTileAtPosition(nextPosition);

            if (nextTile != null && nextTile.IsWalkable && !nextTile.IsOccupied)
            {
                nextTile.IsOccupied = true;
                Vector2Int moveDirection = nextPosition - startTile.Position;

                if (moveDirection != enemy.currentDirection)
                {
                    enemy.StartCoroutine(FaceDirection(moveDirection));
                    enemy.currentDirection = moveDirection;
                }

                enemy.StartCoroutine(MoveToPosition(nextPosition));
            }
            else
            {
                Vector2Int[] alternativeDirections;
                if (nextPosition == startTile.Position + Vector2Int.right || nextPosition == startTile.Position + Vector2Int.left)
                {
                    alternativeDirections = new Vector2Int[] { Vector2Int.up, Vector2Int.down };
                }
                else
                {
                    alternativeDirections = new Vector2Int[] { Vector2Int.right, Vector2Int.left };
                }

                foreach (Vector2Int direction in alternativeDirections)
                {
                    Vector2Int newPosition = startTile.Position + direction;
                    Tile newTile = TileGenerator.Instance.GetTileAtPosition(newPosition);
                    if (newTile != null && newTile.IsWalkable && !newTile.IsOccupied)
                    {
                        newTile.IsOccupied = true;
                        Vector2Int moveDirection = newPosition - startTile.Position;

                        if (moveDirection != enemy.currentDirection)
                        {
                            enemy.StartCoroutine(FaceDirection(moveDirection));
                            enemy.currentDirection = moveDirection;
                        }

                        enemy.StartCoroutine(MoveToPosition(newPosition));
                        return;
                    }
                }
                Debug.Log("No alternative path found");
            }
        }
        else
        {
            Debug.Log("No alternative path found");
        }
    }

    private bool IsNextTileOccupiedByAnotherEnemy(Vector2Int position)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            Vector2Int adjacentPosition = position + direction;
            Tile adjacentTile = TileGenerator.Instance.GetTileAtPosition(adjacentPosition);
            if (adjacentTile != null && adjacentTile.IsOccupied)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2Int FindClosestWalkableTile(Tile startTile, Tile targetTile)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int closestPosition = startTile.Position;
        float closestDistance = float.MaxValue;

        foreach (Vector2Int direction in directions)
        {
            Vector2Int newPosition = startTile.Position + direction;
            Tile newTile = TileGenerator.Instance.GetTileAtPosition(newPosition);
            if (newTile != null && newTile.IsWalkable && !newTile.IsOccupied)
            {
                float distance = Vector2Int.Distance(newPosition, targetTile.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = newPosition;
                }
            }
        }

        return closestPosition;
    }

    private List<Tile> FindAlternativePath(Tile startTile, Tile targetTile)
    {
        List<Tile> alternativePath = TileGenerator.Instance.FindPath(startTile, targetTile);
        if (alternativePath != null && alternativePath.Count > 1)
        {
            Vector2Int nextPosition = alternativePath[1].Position;
            Tile nextTile = TileGenerator.Instance.GetTileAtPosition(nextPosition);
            if (nextTile != null && nextTile.IsWalkable && !nextTile.IsOccupied)
            {
                return alternativePath;
            }
        }
        return null;
    }

    private Vector2Int FindRandomTileTowardsPlayer(Tile startTile, Tile targetTile)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int newPosition = startTile.Position + direction;
            Tile newTile = TileGenerator.Instance.GetTileAtPosition(newPosition);
            if (newTile != null && newTile.IsWalkable && !newTile.IsOccupied)
            {
                possiblePositions.Add(newPosition);
            }
        }

        if (possiblePositions.Count > 0)
        {
            possiblePositions.Sort((a, b) => Vector2Int.Distance(a, targetTile.Position).CompareTo(Vector2Int.Distance(b, targetTile.Position)));
            return possiblePositions[0];
        }

        return startTile.Position;
    }

    private IEnumerator FaceDirection(Vector2Int direction)
    {
        Vector3 targetDirection = Vector3.zero;
        if (direction == Vector2Int.right)
        {
            targetDirection = Vector3.right;
        }
        else if (direction == Vector2Int.left)
        {
            targetDirection = Vector3.left;
        }
        else if (direction == Vector2Int.up)
        {
            targetDirection = Vector3.forward;
        }
        else if (direction == Vector2Int.down)
        {
            targetDirection = Vector3.back;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        while (Quaternion.Angle(enemy.transform.rotation, targetRotation) > 0.1f)
        {
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 10);
            yield return null;
        }
        enemy.transform.rotation = targetRotation;
    }

    private IEnumerator MoveToPosition(Vector2Int position)
    {
        enemy.isMoving = true;

        Vector3 targetPosition = new Vector3(position.x, 0.5f, position.y);
        Vector2Int previousPosition = new Vector2Int((int)enemy.transform.position.x, (int)enemy.transform.position.z);

        while (Vector3.Distance(enemy.transform.position, targetPosition) > 0.1f)
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, Time.deltaTime * 5);
            yield return null;
        }
        enemy.transform.position = targetPosition;

        Tile previousTile = TileGenerator.Instance.GetTileAtPosition(previousPosition);
        Tile targetTile = TileGenerator.Instance.GetTileAtPosition(position);

        if (targetTile != null)
        {
            Debug.Log("Tile Is Unavailable");
            targetTile.SetWalkable(false);
            targetTile.IsOccupied = true;
        }

        TileGenerator.Instance.ResetTileState(previousTile);

        enemy.isMoving = false;
    }
}