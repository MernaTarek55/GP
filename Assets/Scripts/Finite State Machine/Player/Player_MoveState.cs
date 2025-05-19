using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();
        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.playerMove);
    }
}
