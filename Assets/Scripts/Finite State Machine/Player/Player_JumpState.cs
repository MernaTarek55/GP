
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
            player.animator.Update(0f);
            int rocketFlyHash = Animator.StringToHash("RocketFly");
            player.animator.Play(rocketFlyHash);
            player.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            player.hasJumped = true;
        }

        player.animator.SetBool("IsFlying", true);
        player.animator.SetBool("Grounded", false);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGrounded && player.rb.linearVelocity.y < 0)
        {
            player.animator.SetBool("IsFlying", true);
        }

        if (player.IsGrounded && player.hasJumped)
        {
            player.hasJumped = false;
            if (player.healthComponent.IsDead())
            {
                stateMachine.ChangeState(player.playerDeath);
            }
            //else if (player.DeadEyePressed)
            //{
            //    stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
            //}
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
