using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolGuardaState : BaseState<GuardaFSM.AIState>
{
    // Start is called before the first frame update
    public PatrolGuardaState(GuardaFSM.AIState key) : base(key)
    {
        StateKey = key;
    }

    public override void UpdateState()
    {
        // Patrulhar normalmente
        if (((GuardaFSM)MyFsm).patrolPoints.Count > 0)
        {
            Transform patrolTarget = ((GuardaFSM)MyFsm).patrolPoints[((GuardaFSM)MyFsm).currentPatrolIndex];
            ((GuardaFSM)MyFsm).MoveTowards(patrolTarget.position);

            if (Vector3.Distance(((GuardaFSM)MyFsm).transform.position, patrolTarget.position) < 0.2f)
            {
                ((GuardaFSM)MyFsm).currentPatrolIndex = (((GuardaFSM)MyFsm).currentPatrolIndex + 1) % ((GuardaFSM)MyFsm).patrolPoints.Count;
            }
        }
        base.UpdateState();
    }

    public override GuardaFSM.AIState GetNextState()
    {
        if (((GuardaFSM)MyFsm).currentHealth < ((GuardaFSM)MyFsm).maxHealth)
        {
            return GuardaFSM.AIState.Return;
        }

        if (((GuardaFSM)MyFsm).targetEnemy)
        {
            if (((GuardaFSM)MyFsm).IsLowHealth()) return GuardaFSM.AIState.Flee;
            else return GuardaFSM.AIState.Chase;
        }
        
        // Mantem o mesmo
        return StateKey;
    }
}
