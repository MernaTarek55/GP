using UnityEngine;

public class Player_MoveState : EntityState
{
    private float walkTimer = 0f;
    private float timeToRun = 2f; // Time in seconds to start running

    public Player_MoveState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        // Stop moving → go idle
        if (player.MoveInput.sqrMagnitude < 0.01f)
        {
            walkTimer = 0f; // reset walk timer
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
            return;
        }

        // Jump
        if (player.JumpPressed)
        {
            stateMachine.ChangeState(new Player_JumpState(stateMachine, "Jump", player));
            return;
        }

        // Dead
        if (player.healthComponent.IsDead())
        {
            stateMachine.ChangeState(player.playerDeath);
            return;
        }

        // Dead Eye Mode
        if (player.DeadEyePressed)
        {
            stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
            return;
        }

        // Update walk timer
        if (player.MoveInput.sqrMagnitude > 0.01f)
        {
            walkTimer += Time.deltaTime;
        }
        else
        {
            walkTimer = 0f;
        }

        // Determine if player should run
        bool isRunning = walkTimer >= timeToRun;
        float currentMaxSpeed = isRunning ? player.runSpeed : player.walkSpeed;

        // Calculate direction
        Vector3 camRight = player.mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDirection = camRight * player.MoveInput.x;

        // Target velocity
        float inputMagnitude = Mathf.Clamp01(player.MoveInput.magnitude);
        float curvedSpeed = currentMaxSpeed * player.movementCurve.Evaluate(inputMagnitude);
        Vector3 targetVelocity = moveDirection.normalized * curvedSpeed;

        // Accelerate/decelerate
        player.currentVelocity = Vector3.MoveTowards(
            player.currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > 0 ? player.acceleration : player.deceleration) * Time.deltaTime
        );

        // Apply movement
        player.rb.MovePosition(player.rb.position + player.currentVelocity * Time.deltaTime);

        // Rotate if moving
        if (!player.IsShooting && player.currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.currentVelocity);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }

        // Animate with Blend Tree
        float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
        float signedSpeed = Mathf.Sign(player.MoveInput.x) * speedRatio * 2f; // -2 to 2 range
        player.animator.SetFloat("Speed", signedSpeed);
    }
}
