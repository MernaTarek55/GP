using UnityEngine;

public class Player_JumpState : EntityState
{
    private readonly bool hasJumped = false;

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
    }

    public override void Update()
    {
        base.Update();

        if (player.IsGrounded && player.hasJumped)
        {
            player.hasJumped = false;

            if (player.MoveInput.sqrMagnitude > 0.01f)
            {
                stateMachine.ChangeState(new Player_MoveState(stateMachine, "Walk", player));
            }
            else if (player.DeadEyePressed)
            {
                stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
            }
            else if (player.healthComponent.IsDead())
            {
                stateMachine.ChangeState(player.playerDeath);
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

        // player.verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        //player.animator.SetBool("Jump" , false);
        ////player.Animator.SetTrigger("Grounded");
        //player.animator.SetBool("Grounded" , true);
    }
}
