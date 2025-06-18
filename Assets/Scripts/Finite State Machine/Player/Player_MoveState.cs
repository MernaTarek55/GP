
using UnityEngine;

public class Player_MoveState : EntityState
{
    private float timeToRun = 2f;
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

        if (player.DeadEyePressed)
        {
            stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
            return;
        }

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

        player.currentVelocity = Vector3.MoveTowards(
            player.currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > 0 ? player.acceleration : player.deceleration) * Time.deltaTime
        );

        player.rb.MovePosition(player.rb.position + player.currentVelocity * Time.deltaTime);

        if (!player.IsShooting && player.currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.currentVelocity);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }

        float speedRatio = player.currentVelocity.magnitude / player.runSpeed;
        float signedSpeed = Mathf.Sign(player.MoveInput.x) * speedRatio * 2f;
        player.animator.SetFloat("Speed", signedSpeed);
    }

    public void ForceRun()
    {
        forceRun = true;
    }
}
