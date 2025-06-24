using System.Collections;
using UnityEngine;

public abstract class Player_GroundedState : EntityState
{
    protected Player_GroundedState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player) { }

    protected void HandleJump()
    {
        if (player.JumpPressed && player.IsGrounded && !player.hasJumped)
        {
            player.rb.AddForce(Vector3.up * 6f, ForceMode.Impulse);
            //player.animator.SetBool("isJumping", true);
            player.animator.SetTrigger("Jump");
            //player.animator.CrossFade("Jump", 0.1f);
            player.animator.SetFloat("Speed", 0f);
            player.hasJumped = true;

            Debug.Log("✅ Jump triggered and animation should now play.");
        }

        if (player.IsGrounded && player.hasJumped)
        {
            //player.animator.SetBool("isJumping", false);
            player.hasJumped = false;

            Debug.Log("🟢 Landed, jump reset.");
        }
    }

    public override void Update()
    {
        base.Update();

        HandleJump();

        //if (!player.IsGrounded && player.hasJumped)
        //{
        //    player.animator.Play("Fall");
        //}

        if (player.IsGrounded && player.hasJumped)
        {
            //player.animator.SetBool("isJumping", false);
            //player.animator.ResetTrigger("Jump");
            player.hasJumped = false;
        }
    }
}