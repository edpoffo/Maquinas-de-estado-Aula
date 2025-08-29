using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StateMachineManager<EState> : MonoBehaviour where  EState : Enum
{
    protected abstract Dictionary<EState, BaseState<EState>> States { get; set; } // All states from the State Machine
    protected BaseState<EState> CurrentState;  // Current active state
    protected abstract EState StartingStateKey { get; }

    private void TransitionToState(EState nextStateKey)
    {
        CurrentState.ExitState();
        CurrentState = States[nextStateKey];
        CurrentState.EnterState();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = States[StartingStateKey];
        foreach (var state in States)
        {
            state.Value.MyFsm = this;
        }
        CurrentState.EnterState();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        var nextStateKey = CurrentState.GetNextState();
        if (nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();    
        }
        else
        {
            TransitionToState(nextStateKey);
        }
        
    }

    // FixedUpdate is called every x seconds (Normally 0.04s)
    void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }

    private void LateUpdate()
    {
        CurrentState.LateUpdate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CurrentState.OnTriggerEnter2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CurrentState.OnTriggerStay2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CurrentState.OnTriggerExit2D(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentState.OnCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CurrentState.OnCollisionStay2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CurrentState.OnCollisionExit2D(collision);
    }
}
