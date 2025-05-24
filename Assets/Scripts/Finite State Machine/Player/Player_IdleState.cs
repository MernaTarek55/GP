
using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetFloat("Speed", 0f);
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.sqrMagnitude > 0.01f)
            stateMachine.ChangeState(new Player_MoveState(stateMachine, "Walk", player));
        else if (player.JumpPressed)
            stateMachine.ChangeState(new Player_JumpState(stateMachine, "Jump", player));
        else if (player.DeadEyePressed)
            stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
        if (player.InvisibilityPressed && player.CanUseInvisibility)
        {
            stateMachine.ChangeState(new PlayerInvisibilitySkill(stateMachine, "Invisibility", player));
            return;
        }
    }
}
