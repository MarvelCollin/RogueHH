using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovingState : PlayerState
{
    private Player player;
    private Queue<Vector2Int> pathQueue;
    private bool isMoving;
    private Vector2Int currentDirection;
    private Coroutine moveCoroutine;

    public PlayerMovingState(Player player, Queue<Vector2Int> pathQueue)
    {
        this.player = player;
        this.pathQueue = pathQueue;
        this.isMoving = false;
        this.currentDirection = Vector2Int.right;
    }

    public void Update()
    {
        if (!isMoving && pathQueue.Count > 0)
        {
            player.StartCoroutine(MoveAlongPath());
        }
    }

    public void OnEnter()
    {
        player.AnimationToMoving();
        player.StartCoroutine(player.WaitForAnimationAndChangeState());
    }

    public void OnExit()
    {
        player.AnimationToIdle();
    }

    public void Stop()
    {
        if (moveCoroutine != null)
        {
            player.StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            isMoving = false;
            player.AnimationToIdle();
        }
    }

    public void OnEvent(string eventType)
    {
        if (eventType == "Stop")
        {
            Stop();
        }
    }

    private IEnumerator MoveAlongPath()
    {
        moveCoroutine = player.StartCoroutine(MoveAlongPathInternal());
        yield return moveCoroutine;
    }

    private IEnumerator MoveAlongPathInternal()
    {
        isMoving = true;
        Tile previousTile = null;

        while (pathQueue.Count > 0)
        {
            Vector2Int nextPosition = pathQueue.Dequeue();
            Tile nextTile = TileGenerator.Instance.GetTileAtPosition(nextPosition);

            if (nextTile == null || !nextTile.IsWalkable || IsTileOccupied(nextTile))
            {
                Debug.LogWarning("Next tile is not walkable.");
                isMoving = false;
                yield break;
            }
            Vector2Int direction = nextPosition - new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z);

            string directionString = GetDirectionString(direction);
            if (directionString != null)
            {
                yield return FaceDirection(directionString);
                currentDirection = direction;
            }

            yield return MoveToPosition(nextPosition);

            if (previousTile != null)
            {
                previousTile.SetWalkable(true);
            }

            previousTile = nextTile;
            if (TileGenerator.Instance.IsAnyEnemyInAggroState())
            {
                player.AnimationToIdle();
                pathQueue.Clear();
            }
            nextTile.SetWalkable(false);
        }

        isMoving = false;
        if (TileGenerator.Instance.IsAnyEnemyInAggroState())
        {
            TileGenerator.Instance.setEnemyTurn();
        }
        TileGenerator.Instance.ClearPathHighlight();
        TileGenerator.Instance.setEnemyTurn();
        player.SetState(new PlayerIdleState(player));
    }

    private string GetDirectionString(Vector2Int direction)
    {
        if (direction == Vector2Int.right)
        {
            return "right";
        }
        else if (direction == Vector2Int.left)
        {
            return "left";
        }
        else if (direction == Vector2Int.up)
        {
            return "forward";
        }
        else if (direction == Vector2Int.down)
        {
            return "down";
        }
        return null;
    }

    private IEnumerator FaceDirection(string direction)
    {
        Vector3 targetDirection = Vector3.zero;
        if (direction == "right")
        {
            targetDirection = Vector3.right;
        }
        else if (direction == "left")
        {
            targetDirection = Vector3.left;
        }
        else if (direction == "forward")
        {
            targetDirection = Vector3.forward;
        }
        else if (direction == "down")
        {
            targetDirection = Vector3.back;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        while (Quaternion.Angle(player.transform.rotation, targetRotation) > 0.1f)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 10);
            yield return null;
        }
        player.transform.rotation = targetRotation;
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

    private IEnumerator MoveToPosition(Vector2Int position)
    {
        Tile currentTile = TileGenerator.Instance.GetTileAtPosition(new Vector2Int((int)player.transform.position.x, (int)player.transform.position.z));
        if (currentTile != null)
        {
            currentTile.SetWalkable(true);
            currentTile.IsOccupied = false;
        }

        Vector3 targetPosition = new Vector3(position.x, 0.5f, position.y);
        while (Vector3.Distance(player.transform.position, targetPosition) > 0.1f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, Time.deltaTime * 5);
            yield return null;
        }
        player.transform.position = targetPosition;

        Tile targetTile = TileGenerator.Instance.GetTileAtPosition(position);
        if (targetTile != null)
        {
            targetTile.SetWalkable(false);
            targetTile.IsOccupied = true;
        }
    }
}