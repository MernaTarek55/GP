using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
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

        Vector3 nextPosition = player.rb.position + player.currentVelocity * Time.deltaTime;
        nextPosition.z = 0f; // Enforce flat movement
        player.rb.MovePosition(nextPosition);
        //player.rb.MovePosition(player.rb.position + player.currentVelocity * Time.deltaTime);

        if (!player.IsShooting && player.currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.currentVelocity);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }

        if (!player.hasJumped)
        {
            float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
            float signedSpeed = Mathf.Sign(player.MoveInput.x) * speedRatio * 2f;
            player.animator.SetFloat("Speed", signedSpeed);
        }

        if (player.IsGrounded)
        {
            float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
            float signedSpeed = Mathf.Sign(player.MoveInput.x) * speedRatio * 2f;
            player.animator.SetFloat("Speed", signedSpeed);
        }
    }

    public void ForceRun()
    {
        forceRun = true;
    }
}