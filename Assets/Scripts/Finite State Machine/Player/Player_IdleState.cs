public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.WalkTimer = 0f;
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGrounded)
            return;

        player.animator.SetFloat("Speed", 0f);

        if (player.healthComponent.IsDead())
            stateMachine.ChangeState(player.playerDeath);
        else if (player.MoveInput.sqrMagnitude > 0.01f)
            stateMachine.ChangeState(new Player_MoveState(stateMachine, "Move", player));
        else if (player.JumpPressed)
            stateMachine.ChangeState(new Player_JumpState(stateMachine, "Jump", player));
        //else if (player.DeadEyePressed)
        //    stateMachine.ChangeState(new Player_DeadEyeStateTest1(stateMachine, "DeadEye", player));
    }
}
