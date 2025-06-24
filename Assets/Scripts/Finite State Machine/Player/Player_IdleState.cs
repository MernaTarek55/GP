using UnityEngine;
public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player) { }

    public override void Enter()
    {
        base.Enter();
        player.WalkTimer = 0f;
        player.animator.SetFloat("Speed", 0f); // Always reset to 0 when entering Idle
    }

    public override void Update()
    {
        base.Update();

        // Keep ensuring Speed stays 0 in Idle
        player.animator.SetFloat("Speed", 0f);

        if (player.healthComponent.IsDead() || player.transform.position.y <= -35f)
            stateMachine.ChangeState(player.playerDeath);
        else if (player.MoveInput.sqrMagnitude > 0.01f)
            stateMachine.ChangeState(new Player_MoveState(stateMachine, "Move", player));
    }
}