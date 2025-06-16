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


        //Vector3 camForward = player.mainCamera.transform.forward;
        //Vector3 camRight = player.mainCamera.transform.right;
        ////camForward.y = 0; 
        //camRight.y = 0;
        ////camForward.Normalize(); 
        //camRight.Normalize();

        ////Vector3 moveDirection = (camForward * player.MoveInput.y) + (camRight * player.MoveInput.x);
        //Vector3 moveDirection = camRight * player.MoveInput.x;

        ////player.animator.SetFloat("Speed", player.MoveInput.magnitude);
        //player.animator.SetFloat("Speed", Mathf.Abs(player.MoveInput.x));
        ////player.rb.MovePosition(player.rb.position + (moveDirection * player.WalkSpeed * Time.deltaTime));
        //float inputMagnitude = Mathf.Clamp01(player.MoveInput.magnitude);
        //float curveValue = player.movementCurve.Evaluate(inputMagnitude);
        //float curvedSpeed = player.WalkSpeed * curveValue;

        //player.rb.MovePosition(player.rb.position + (moveDirection * curvedSpeed * Time.deltaTime));


        //if (!player.IsShooting && moveDirection.sqrMagnitude > 0.01f)
        //{
        //    float rotationCurve = player.movementCurve.Evaluate(inputMagnitude);
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        //    player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, rotationCurve * player.RotateSpeed * Time.deltaTime));

        //    //player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        //}

        // Input and directions
        Vector3 camRight = player.mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDirection = camRight * player.MoveInput.x;

        // Target velocity based on curve
        float inputMagnitude = Mathf.Clamp01(player.MoveInput.magnitude);
        float curvedSpeed = player.maxSpeed * player.movementCurve.Evaluate(inputMagnitude);
        Vector3 targetVelocity = moveDirection.normalized * curvedSpeed;

        // Accelerate or decelerate smoothly
        player.currentVelocity = Vector3.MoveTowards(
            player.currentVelocity,
            targetVelocity,
            (targetVelocity.magnitude > 0 ? player.acceleration : player.deceleration) * Time.deltaTime
        );

        // Apply movement
        player.rb.MovePosition(player.rb.position + player.currentVelocity * Time.deltaTime);

        // Smooth rotation if moving
        if (!player.IsShooting && player.currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.currentVelocity);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, player.RotateSpeed * Time.deltaTime));
        }

        // Animate
        float speedRatio = player.currentVelocity.magnitude / player.maxSpeed;
        player.animator.SetFloat("Speed", speedRatio * 1.5f); // 1.5 = speed multiplier

    }
}
