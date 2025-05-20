
using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.Input.move.magnitude > 0.1f)
        {
            stateMachine.ChangeState(player.playerMove);
        }
        // Check for jump input
        else if (player.Input.jump && player.isGrounded)
        {
            stateMachine.ChangeState(player.playerJump);
        }
    }
}
