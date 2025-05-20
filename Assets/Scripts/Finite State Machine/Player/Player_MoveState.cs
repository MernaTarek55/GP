using UnityEngine;

public class Player_MoveState : EntityState
{
    float rotationVelocity;
    public Player_MoveState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        // Get input
        Vector2 input = player.Input.move;

        // Calculate movement direction
        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;

        // Rotate towards movement direction
        if (moveDir != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(
                player.transform.eulerAngles.y,
                targetRotation,
                ref rotationVelocity,
                player.rotationSmoothTime
            );
            player.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        // Apply movement
        float targetSpeed = player.Input.sprint ? player.sprintSpeed : player.moveSpeed;
        player.currentSpeed = Mathf.Lerp(player.currentSpeed, targetSpeed, Time.deltaTime * 10f);

        player.Controller.Move(moveDir * player.currentSpeed * Time.deltaTime);

        // Handle state transitions
        if (input.magnitude < 0.1f)
        {
            stateMachine.ChangeState(player.playerIdle);
        }
        else if (player.Input.jump && player.isGrounded)
        {
            stateMachine.ChangeState(player.playerJump);
        }
    }
}
