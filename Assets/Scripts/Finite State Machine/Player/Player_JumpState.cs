using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }
    public override void Enter()
    {
        base.Enter();

        // When its time to use animation for jump
    }

    public override void Update()
    {
        base.Update();

        if (player.IsGrounded)
        {
            if (player.MoveInput.magnitude > 0.1f)
            {
                stateMachine.ChangeState(player.playerMove);
            }
            else
            {
                stateMachine.ChangeState(player.playerIdle);
            }
        }
    }
}
