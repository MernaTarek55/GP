
using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(player.playerMove);
        }
        else if (player.JumpTriggered && player.IsGrounded)
        {
            stateMachine.ChangeState(player.playerJump);
        }
    }
}
