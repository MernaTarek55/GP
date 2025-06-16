using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player.IsGrounded && !player.hasJumped)
        {
            player.animator.SetTrigger("Jump");
            player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            player.hasJumped = true;
        }

        // At jump, player is airborne, so IsFlying = true
        player.animator.SetBool("IsFlying", true);
        player.animator.SetBool("Grounded", false);
    }

    public override void Update()
    {
        base.Update();

        // Check if player is falling
        if (!player.IsGrounded && player.rb.linearVelocity.y < 0)
        {
            player.animator.SetBool("IsFlying", true);
        }

        if (player.IsGrounded && player.hasJumped)
        {
            player.hasJumped = false;

            stateMachine.ChangeState(
                player.healthComponent.IsDead() ? player.playerDeath :
                player.DeadEyePressed ? new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player) :
                player.MoveInput.sqrMagnitude > 0.01f ? new Player_MoveState(stateMachine, "Walk", player) :
                new Player_IdleState(stateMachine, "Idle", player)
            );
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Reset animation parameters
        player.animator.SetBool("IsFlying", false);
        player.animator.SetBool("Grounded", true);
        player.animator.ResetTrigger("Jump");
    }
}
