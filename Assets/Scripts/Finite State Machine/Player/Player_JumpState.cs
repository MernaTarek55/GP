using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }
    public override void Enter()
    {
        base.Enter();

        player.animator.SetTrigger("Jump");
        player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    public override void Update()
    {
        base.Update();

        // Placeholder: Switch to Idle or Walk based on grounded check
        if (player.MoveInput.sqrMagnitude > 0.01f)
            stateMachine.ChangeState(new Player_MoveState(stateMachine, "Walk", player));
        else
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
    }
}
