
using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player.IsGrounded && !player.hasJumped)
        {
            player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            player.hasJumped = true;
        }
        player.animator.SetBool("IsJumping", true); // Use if needed to control transitions
        player.dust.Play();
    }

    public override void Update()
    {
        base.Update();

        // Update vertical velocity for the blend tree
        float verticalVelocity = player.rb.linearVelocity.y;
        player.animator.SetFloat("VerticalVelocity", verticalVelocity);

        if (player.IsGrounded && player.hasJumped)
        {
            player.hasJumped = false;

            if (player.healthComponent.IsDead())
            {
                stateMachine.ChangeState(player.playerDeath);
            }
            else if (player.MoveInput.sqrMagnitude > 0.01f)
            {
                var moveState = new Player_MoveState(stateMachine, "Move", player);
                if (player.WasRunningBeforeJump)
                    moveState.ForceRun();
                stateMachine.ChangeState(moveState);
            }
            else
            {
                stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
            }
        }
    }


    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("IsFlying", false);
        player.animator.SetBool("Grounded", true);
        //player.animator.ResetTrigger("Jump");
    }
}
