using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.magnitude < 0.1f)
        {
            stateMachine.ChangeState(player.playerIdle);
        }
        else if (player.JumpTriggered && player.IsGrounded)
        {
            stateMachine.ChangeState(player.playerJump);
        }
    }
}
