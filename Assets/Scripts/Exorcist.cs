using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Exorcist : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum State
    {
        Idle,
        Chasing,
        Guarding,
        Attacking,
    }
    private State currState;
    private Vector3 currTargetLoc;
    private float nextStateChange;
    private bool isAttacking;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float viewDistance = 30f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private HP hp;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject attack;
    void Start()
    {
        currState = State.Idle;
        hp.OnDie += Die;
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (canSeePlayer())
        {
            OnStateToChasing();
        }
        else if (Time.time >= nextStateChange && !isAttacking)
        {
            DecideNextState();
        }

        if (currState == State.Chasing)
        {
            agent.SetDestination(Sinner.instance.transform.position);
        }
        else
        {
            agent.SetDestination(currTargetLoc);
        }

        if (Sinner.instance != null)
        {
            float distanceToSinner = Vector3.Distance(this.transform.position, Sinner.instance.transform.position);
            if (distanceToSinner <= 3f && !isAttacking)
            {
                AttackPlayer();
            }
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Attacks player");
        isAttacking = true;
        currTargetLoc = this.transform.position;
        agent.SetDestination(currTargetLoc);
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(attack, this.transform.position + this.transform.forward * 2f + Vector3.up, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        DecideNextState();
    }

    void DecideNextState()
    {
        int r = Random.Range(0, 3);
        if (r == 0)
        {
            OnStateToIdle();
        }
        else if (r == 1)
        {
            OnStateToChasing();
        } else
        {
            OnStateToGuarding();
        }
    }

    void OnStateToIdle()
    {
        currState = State.Idle;
        int r = Random.Range(0, MazeMaker.Instance.width * MazeMaker.Instance.length);
        currTargetLoc = MazeMaker.Instance.GetCellLocation(r);
        agent.SetDestination(currTargetLoc);
        nextStateChange = Time.time + Random.Range(3f, 6f);
    }

    void OnStateToChasing()
    {
        currState = State.Chasing;
        agent.SetDestination(Sinner.instance.transform.position);
        currTargetLoc = Sinner.instance.transform.position;
        nextStateChange = Time.time + Random.Range(3f, 6f);
    }

    void OnStateToGuarding()
    {
        currState = State.Guarding;
        int r = Random.Range(0, MazeMaker.Instance.RitualPositions.Count);
        currTargetLoc = MazeMaker.Instance.RitualPositions[r];
        agent.SetDestination(currTargetLoc);
        nextStateChange = Time.time + Random.Range(3f, 6f);
    }
    
    bool canSeePlayer()
    {
        Transform player = Sinner.instance.transform;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= viewDistance)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distance, obstacleMask))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
}
