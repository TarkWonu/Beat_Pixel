using UnityEngine;


public class StateMachine
{
    public State currentState{get; private set;}

    public StateMachine(State defaultState)
    {
        currentState = defaultState;
        currentState.OnEnter();
    }

    public void ChangeState(State newState)
    {
        if(newState == currentState) return;

        currentState?.OnExit();

        currentState = newState;
        currentState.OnEnter();
    }

    public void UpdateState()
    {
        currentState?.OnUpdate();
    }



}