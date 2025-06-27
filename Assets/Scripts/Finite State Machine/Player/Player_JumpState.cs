using UnityEngine;

public class Player_JumpState : EntityState
{
    private float jumpDuration;
    private float maxJumpTime = 2f; // Failsafe timeout

    public Player_JumpState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        jumpDuration = 0f;

        if (player.IsGrounded && !player.hasJumped)
        {
            player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            player.hasJumped = true;
        }

        player.animator.SetBool("IsJumping", true);
        player.dust.Play();
    }

    public override void Update()
    {
        base.Update();
        jumpDuration += Time.deltaTime;

        // Update vertical velocity for animation blend tree
        float verticalVelocity = player.rb.linearVelocity.y;
        player.animator.SetFloat("VerticalVelocity", verticalVelocity);

        // Return to proper state when grounded
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
        // Failsafe: force back to idle if stuck too long
        else if (jumpDuration > maxJumpTime)
        {
            Debug.LogWarning("Jump timeout reached, returning to Idle.");
            player.hasJumped = false;
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("IsJumping", false);
        player.animator.SetBool("IsFlying", false);
        player.animator.SetBool("Grounded", true);
    }
}
