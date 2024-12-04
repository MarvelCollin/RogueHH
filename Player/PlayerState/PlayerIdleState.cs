using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private Player player;

    public PlayerIdleState(Player player)
    {
        this.player = player;
    }

    public void Update()
    {
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
        player.Animator.ResetTrigger("isIdle");
    }

    public void OnEvent(string eventType)
    {
        // Handle events if needed
    }
}