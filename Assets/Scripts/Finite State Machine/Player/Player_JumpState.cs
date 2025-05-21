using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }
    public override void Enter()
    {
        base.Enter();

        player.verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        player.Animator.SetTrigger("Jump");
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded && player.verticalVelocity < 0)
        {
            if (player.Input.move.magnitude > 0.1f)
                stateMachine.ChangeState(player.playerMove);
            else
                stateMachine.ChangeState(player.playerIdle);
        }
    }
    public override void Exit()
    {
        base.Exit();

       // player.verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        player.Animator.SetBool("Jump" , false);
        //player.Animator.SetTrigger("Grounded");
        player.Animator.SetBool("Grounded" , true);
    }
}
