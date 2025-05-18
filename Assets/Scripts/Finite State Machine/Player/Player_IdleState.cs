using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Update()
    {
        base.Update();

    }
}
