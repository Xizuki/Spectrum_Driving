using System.Collections;
using System.Collections.Generic;

public class FSM 
{
    Dictionary<string, List<State>> statesDictionary;
    List<State> states;
    State currentStates;

    public FSM()
    {
        states = new List<State>();
    }

    public void AddState(State state)
    { 
        states.Add(state);
    }

    public State GetCurrentState()
    {
        return currentStates;
    }

    public void SetCurrentState(int id) 
    {
        State st = states[id];
        if (st == null) { return; }
        currentStates = st;
    }
}

public class State
{
    FSM fsm;
    protected int id;

    public State(FSM fsm)
    { 
        this.fsm = fsm; 
    }
}