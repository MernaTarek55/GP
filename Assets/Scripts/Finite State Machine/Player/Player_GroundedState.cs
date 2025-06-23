using UnityEngine;

public abstract class Player_GroundedState : EntityState
{
    protected Player_GroundedState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player) { }

    protected void HandleJump()
    {
        if (player.JumpPressed && player.IsGrounded && !player.hasJumped)
        {
            player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            player.animator.Play("Jump");
            player.hasJumped = true;
        }
    }

    public override void Update()
    {
        base.Update();

        HandleJump();

        if (!player.IsGrounded && player.hasJumped)
        {
            player.animator.Play("Fall");
        }

        if (player.IsGrounded && player.hasJumped)
        {
            player.hasJumped = false;
        }
    }
}