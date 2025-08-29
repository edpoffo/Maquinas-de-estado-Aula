using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachineAI : MonoBehaviour
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

    private int currentPatrolIndex = 0;
    private Transform targetEnemy;
    private AIState currentState = AIState.Patrol;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) currentHealth -= 10;
        
        // Checa morte antes de processar
        if (currentHealth <= 0 && currentState != AIState.Dead)
        {
            ChangeState(AIState.Dead);
        }

        // Atualiza inimigo alvo
        targetEnemy = GetNearestEnemy();
        if(targetEnemy) Debug.Log("ENEMY FOUND");

        // Executa comportamento baseado no estado atual
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Flee:
                FleeBehavior();
                break;
            case AIState.Return:
                ReturnBehavior();
                break;
            case AIState.Recover:
                RecoverBehavior();
                break;
            case AIState.Dead:
                DeadBehavior();
                break;
        }
    }

    // ---- LÓGICAS DE ESTADO ----
#region LogicasEstado
    void PatrolBehavior()
    {
        if (currentHealth < maxHealth)
        {
            ChangeState(AIState.Return);
            return;
        }

        if (targetEnemy)
        {
            if (IsLowHealth()) ChangeState(AIState.Flee);
            else ChangeState(AIState.Chase);
            return;
        }

        // Patrulhar normalmente
        if (patrolPoints.Count > 0)
        {
            Transform patrolTarget = patrolPoints[currentPatrolIndex];
            MoveTowards(patrolTarget.position);

            if (Vector3.Distance(transform.position, patrolTarget.position) < 0.2f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            }
        }
    }

    void ChaseBehavior()
    {
        if (targetEnemy == null)
        {
            ChangeState(AIState.Patrol);
            return;
        }

        if (IsLowHealth())
        {
            ChangeState(AIState.Flee);
            return;
        }

        float dist = Vector3.Distance(transform.position, targetEnemy.position);
        if (dist <= attackRange)
        {
            ChangeState(AIState.Attack);
            return;
        }

        MoveTowards(targetEnemy.position);
    }

    void AttackBehavior()
    {
        //// Condições para troca de estado (Transições)
        if (targetEnemy == null)
        {
            ChangeState(AIState.Patrol);
            return;
        }

        var dist = Vector3.Distance(transform.position, targetEnemy.position);
        if (dist > attackRange)
        {
            ChangeState(AIState.Chase);
            return;
        }
        
        if (IsLowHealth())
        {
            ChangeState(AIState.Flee);
            return;
        }
        
        // Execução do comportamento
        Debug.Log("Atacando inimigo!"); // Substituir pela lógica de atacar o inimigo de fato
    }

    void FleeBehavior()
    {
        if (targetEnemy == null)
        {
            ChangeState(AIState.Return);
            return;
        }

        if (Vector3.Distance(transform.position, targetEnemy.position) > detectionRange)
        {
            ChangeState(AIState.Return);
        }
        
        FleeFrom(targetEnemy.position);

        
    }

    void ReturnBehavior()
    {
        if (Vector3.Distance(transform.position, originPoint.position) < 0.1f)
        {
            ChangeState(AIState.Recover);
            return;
        }

        if (targetEnemy)
        {
            ChangeState(AIState.Flee);
            return;
        }
        
        MoveTowards(originPoint.position);
    }

    void RecoverBehavior()
    {
        if (currentHealth >= maxHealth)
        {
            ChangeState(AIState.Patrol);
            return;
        }

        if (targetEnemy)
        {
            if(IsLowHealth()) ChangeState(AIState.Flee);
            else ChangeState(AIState.Chase);
            return;
        }
        
        RecoverHealth();
    }

    void DeadBehavior()
    {
        Debug.Log("Personagem morreu!");
        gameObject.SetActive(false);
    }
#endregion

    // ---- MÉTODOS AUXILIARES ----

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

    void MoveTowards(Vector3 targetPos)
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

    bool IsLowHealth()
    {
        return currentHealth <= maxHealth * LowHealthThreshold;
    }

    void ChangeState(AIState newState)
    {
        currentState = newState;
    }
}
