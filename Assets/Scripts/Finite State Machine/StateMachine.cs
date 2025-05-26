

public class StateMachine
{
    public EntityState currentState { get; private set; }

    public void Initalize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (currentState != newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }
            
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }
}
