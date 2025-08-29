using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum
{
    public StateMachineManager<EState> MyFsm;
    
    public BaseState(EState key)
    {
        StateKey = key;
    }

    public EState StateKey { get; protected set; }

    public virtual void EnterState(){}
    public virtual void ExitState(){}
    public abstract EState GetNextState();
    
    // Updates (State stay)
    public virtual void UpdateState(){}
    public virtual void FixedUpdate(){}

    public virtual void LateUpdate()
    {
        Debug.Log("Auau");
    }
    
    // Triggers 2D
    public virtual void OnTriggerEnter2D(Collider2D other){}
    public virtual void OnTriggerExit2D(Collider2D other){}
    public virtual void OnTriggerStay2D(Collider2D other){}
    
    // Collision 2D
    public virtual void OnCollisionEnter2D(Collision2D collision){}
    public virtual void OnCollisionExit2D(Collision2D collision){}
    public virtual void OnCollisionStay2D(Collision2D collision){}
    
}
