using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public StateMachine expStateMachine { get; private set; }
    public Example_IdleState expIdleState { get; private set; }

    private void Awake()
    {
        expStateMachine = new StateMachine();
        expIdleState = new Example_IdleState(expStateMachine, "Example Idle", this);
    }

    private void Start()
    {
        expStateMachine.Initalize(expIdleState);// And Start with it.
    }

    private void Update()
    {
        expStateMachine.currentState.Update();
    }
}
