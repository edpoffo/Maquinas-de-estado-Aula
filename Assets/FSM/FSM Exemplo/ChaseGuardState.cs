using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseGuardState : BaseState<GuardaFSM.AIState>
{
    // Start is called before the first frame update
    public ChaseGuardState(GuardaFSM.AIState key) : base(key)
    {
        StateKey = key;
    }

    public override void UpdateState()
    {
        var stateMachine = (GuardaFSM)MyFsm;
        
        stateMachine.MoveTowards(stateMachine.targetEnemy.position);
    }

    public override GuardaFSM.AIState GetNextState()
    {
        var stateMachine = (GuardaFSM)MyFsm;
        
        if (stateMachine.targetEnemy == null)
        {
            return GuardaFSM.AIState.Patrol;
        }

        if (stateMachine.IsLowHealth())
        {
            return GuardaFSM.AIState.Flee;
        }

        float dist = Vector3.Distance(stateMachine.transform.position, stateMachine.targetEnemy.position);
        if (dist <= stateMachine.attackRange)
        {
            return GuardaFSM.AIState.Attack;
        }

        return StateKey;
    }
}
