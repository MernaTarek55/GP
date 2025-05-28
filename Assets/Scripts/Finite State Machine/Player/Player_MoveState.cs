using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.sqrMagnitude < 0.01f)
        {
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
            return;
        }

        if (player.JumpPressed)
        {
            stateMachine.ChangeState(new Player_JumpState(stateMachine, "Jump", player));
            return;
        }

        if (player.healthComponent.IsDead())
        {
            stateMachine.ChangeState(player.playerDeath);
        }

        if (player.DeadEyePressed)
        {
            stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
            return;
        }


        Vector3 camForward = player.mainCamera.transform.forward;
        Vector3 camRight = player.mainCamera.transform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDirection = (camForward * player.MoveInput.y) + (camRight * player.MoveInput.x);

        player.animator.SetFloat("Speed", player.MoveInput.magnitude);
        player.rb.MovePosition(player.rb.position + (moveDirection * player.WalkSpeed * Time.deltaTime));

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }
    }
}
