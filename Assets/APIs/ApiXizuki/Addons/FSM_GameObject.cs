using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnstileLockedState:State
{
    
    public TurnstileLockedState(FSM fsm): base(fsm)
    {
        id = 0;
    }
}

public class TurnstileUnlockedState : State
{
    public TurnstileUnlockedState(FSM fsm) : base(fsm)
    {
        id = 1;
    }
}
public class TurnStile : MonoBehaviour
{
    FSM turnStileFSM = new FSM();
    // Start is called before the first frame update
    void Start()
    {
        turnStileFSM.AddState(new TurnstileLockedState(turnStileFSM));
        turnStileFSM.AddState(new TurnstileUnlockedState(turnStileFSM));
        turnStileFSM.SetCurrentState(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
