using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuardaFSM : StateMachineManager<GuardaFSM.AIState>
{
    public enum AIState
    {
        Patrol,     // Patrulhando
        Chase,      // Perseguindo inimigo
        Attack,     // Atacando inimigo
        Flee,       // Fugindo com pouca vida
        Return,     // Voltando para origem para curar
        Recover,    // Recuperando vida
        Dead,        // Morto
    }
    
    protected override AIState StartingStateKey { get; } =  AIState.Patrol;
    
    [Header("Configurações")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    public float healthRegenRate = 10f;

    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    private const float LowHealthThreshold = 0.5f;

    [Header("Referências")]
    public Transform originPoint;
    public List<Transform> patrolPoints;

    public int currentPatrolIndex = 0;
    public Transform targetEnemy;
    

    protected override Dictionary<AIState, BaseState<AIState>> States { get; set; } =
        new Dictionary<AIState, BaseState<AIState>>()
        {
            { AIState.Patrol, new PatrolGuardaState(AIState.Patrol)},
            
        };

    protected override void Update()
    {
        targetEnemy = GetNearestEnemy();
        base.Update();
    }

    #region MétodosAdicionais

    Transform GetNearestEnemy()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange).ToList();
        Debug.Log(colliders.Count);
        if (colliders.Count == 0) return null;

        var smallestDistance = detectionRange * 2;
        Collider2D closestCollider = null;
        foreach (var c in colliders)
        {
            var dist = Vector2.Distance(transform.position, c.transform.position);
            if (dist < smallestDistance)
            {
                smallestDistance = dist;
                closestCollider = c;
            }
        }
        
        return closestCollider.transform;
    }

    public void MoveTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    void FleeFrom(Vector3 threatPos)
    {
        Vector3 direction = (transform.position - threatPos).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    void RecoverHealth()
    {
        currentHealth += healthRegenRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
        Debug.Log("Recuperando vida: " + currentHealth);
    }

    public bool IsLowHealth()
    {
        return currentHealth <= maxHealth * LowHealthThreshold;
    }

    #endregion
    
}
