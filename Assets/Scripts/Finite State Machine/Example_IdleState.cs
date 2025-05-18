using UnityEngine;

public class Example_IdleState : EntityState
{
    public Example_IdleState(StateMachine stateMachine, string stateName, ExampleScript exp) : base(stateMachine, stateName, exp)
    {
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.F))
            stateMachine.ChangeState(example.expIdleState);
    }
}
