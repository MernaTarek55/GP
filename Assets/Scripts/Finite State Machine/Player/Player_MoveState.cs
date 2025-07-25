using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    //please
    private float timeToRun = 0.5f;
    private bool forceRun = false;

    public Player_MoveState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player) { }

    public override void Update()
    {
        base.Update();

        if (player.healthComponent.IsDead())
        {
            stateMachine.ChangeState(player.playerDeath);
            return;
        }

        if (player.MoveInput.sqrMagnitude < 0.01f)
        {
            player.WalkTimer = 0f;
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
            return;
        }

        if (!forceRun)
        {
            player.WalkTimer += Time.deltaTime;
        }

        bool isRunning = forceRun || player.WalkTimer >= timeToRun;
        float currentMaxSpeed = isRunning ? player.runSpeed : player.walkSpeed;

        Vector3 camRight = player.mainCamera.transform.right;
        camRight.y = 0f;
        camRight.z = 0f;
        camRight.Normalize();

        Vector3 moveDirection = camRight * player.MoveInput.x;
        float inputMagnitude = Mathf.Clamp01(player.MoveInput.magnitude);
        float curvedSpeed = currentMaxSpeed * player.movementCurve.Evaluate(inputMagnitude);
        Vector3 targetVelocity = moveDirection.normalized * curvedSpeed;

        player.currentVelocity = Vector3.Lerp(
            player.currentVelocity,
            targetVelocity,
            player.movementCurve.Evaluate(inputMagnitude) * Time.deltaTime * player.acceleration
        );

        if (player.MoveInput.sqrMagnitude > 0.01f)
        {
            // Move normally
            float targetXVelocity = player.MoveInput.x * player.maxSpeed;
            float newX = Mathf.MoveTowards(
                player.rb.linearVelocity.x,
                targetXVelocity,
                player.acceleration * Time.deltaTime
            );
            player.SetVelocity(newX, player.rb.linearVelocity.y);
        }


        if (!player.IsShooting && Mathf.Abs(player.currentVelocity.x) > 0.01f)
        {
            float desiredY = player.currentVelocity.x > 0 ? 90f : 270f;
            Quaternion desiredRotation = Quaternion.Euler(0f, desiredY, 0f);

            if (!player.rotating || Quaternion.Angle(player.targetRot, desiredRotation) > 1f)
            {
                player.startRot = player.transform.rotation;
                player.targetRot = desiredRotation;
                player.rotateTimer = 0f;
                player.rotating = true;
            }
        }

        // Apply rotation smoothly
        if (player.rotating)
        {
            player.rotateTimer += Time.deltaTime;
            float t = Mathf.Clamp01(player.rotateTimer / player.rotateDuration);
            float curvedT = player.rotationCurve.Evaluate(t);
            player.transform.rotation = Quaternion.Slerp(player.startRot, player.targetRot, curvedT);
            if(player.IsGrounded) player.dust.Play();
            if (t >= 1f)
                player.rotating = false;
        }


        if (!player.hasJumped && player.IsGrounded)
        {
            float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
            float unsignedSpeed = speedRatio * 2f;
            player.animator.SetFloat("Speed", unsignedSpeed);
        }
    }

    public void ForceRun()
    {
        forceRun = true;
    }
}