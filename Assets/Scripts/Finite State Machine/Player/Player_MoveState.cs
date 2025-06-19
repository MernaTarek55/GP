
using UnityEngine;

public class Player_MoveState : EntityState
{
    private float timeToRun = .5f;
    private bool forceRun = false;

    public Player_MoveState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.sqrMagnitude < 0.01f)
        {
            player.WalkTimer = 0f;
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
            return;
        }

        if (player.JumpPressed)
        {
            player.WasRunningBeforeJump = player.WalkTimer >= timeToRun;
            stateMachine.ChangeState(new Player_JumpState(stateMachine, "Jump", player));
            return;
        }

        if (player.healthComponent.IsDead())
        {
            stateMachine.ChangeState(player.playerDeath);
            return;
        }

        //if (player.DeadEyePressed)
        //{
        //    stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
        //    return;
        //}

        if (!forceRun)
        {
            if (player.MoveInput.sqrMagnitude > 0.01f)
                player.WalkTimer += Time.deltaTime;
            else
                player.WalkTimer = 0f;
        }

        bool isRunning = forceRun || player.WalkTimer >= timeToRun;
        float currentMaxSpeed = isRunning ? player.runSpeed : player.walkSpeed;

        Vector3 camRight = player.mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDirection = camRight * player.MoveInput.x;
        float inputMagnitude = Mathf.Clamp01(player.MoveInput.magnitude);
        float curvedSpeed = currentMaxSpeed * player.movementCurve.Evaluate(inputMagnitude);
        Vector3 targetVelocity = moveDirection.normalized * curvedSpeed;

        // EaseInOut curve value based on input magnitude (0 to 1)
        float easeValue = player.movementCurve.Evaluate(inputMagnitude); // e.g. EaseInOut(0,0)-(1,1)

        // Smooth using Lerp
        player.currentVelocity = Vector3.Lerp(
            player.currentVelocity,
            targetVelocity,
            easeValue * Time.deltaTime * player.acceleration
        );

        player.rb.MovePosition(player.rb.position + player.currentVelocity * Time.deltaTime);

        if (!player.IsShooting && player.currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.currentVelocity);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }

        if (player.IsGrounded)
        {
            float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
            float signedSpeed = Mathf.Sign(player.MoveInput.x) * speedRatio * 2f;
            player.animator.SetFloat("Speed", signedSpeed);
        }
        else
        {
            player.animator.SetFloat("Speed", 0f);
        }
    }

    public void ForceRun()
    {
        forceRun = true;
    }
}
