using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    [SerializeField] private Collider attack;
    [SerializeField] private VisualEffect slash;
    [SerializeField] private AudioSource walkAudio;
    [SerializeField] private AudioSource hitAudio;

    private float nextAttackTime;
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isDead = false;
    void Start()
    {
        currState = State.Idle;
        hp.OnDie += Die;
        slash.enabled = false;
        hp.OnTakeDamage += (a) => OnTakeDamage(a);
        attack.GetComponent<HitBox>().owner = hp.gameObject;    
    }

    void OnTakeDamage(int damage)
    {
        if (!canSeePlayer())
        {
            Die();
        }
        hitAudio.Play();
        HitPause.Instance.HitStop(.1f);
    }

    void Die()
    {
        isDead = true;
        Sinner.instance.ExtendDarkMagic(5f);
        MazeMaker.Instance.AddPoints(20);
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
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
            if (!walkAudio.isPlaying) walkAudio.Play();
        }
        else
        {
            agent.SetDestination(currTargetLoc);
            if (walkAudio.isPlaying) walkAudio.Stop();
        }

        if (Sinner.instance != null)
        {
            float distanceToSinner = Vector3.Distance(this.transform.position, Sinner.instance.transform.position);
            if (distanceToSinner <= 3f && !isAttacking && Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        Vector3 v = Sinner.instance.transform.position - this.transform.position;
        currTargetLoc = v.normalized * 0.1f + this.transform.position;
        agent.SetDestination(currTargetLoc);
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        slash.enabled = true;
        slash.Play();
        yield return new WaitForSeconds(0.1f);
        attack.enabled = true;
        yield return new WaitForSeconds(2f);
        attack.enabled = false;
        slash.Stop();
        slash.enabled = false;
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
        Debug.Log(gameObject.name + " to Idle");
        currState = State.Idle;
        int r = Random.Range(0, MazeMaker.Instance.width * MazeMaker.Instance.length);
        currTargetLoc = MazeMaker.Instance.GetCellLocation(r);
        agent.SetDestination(currTargetLoc);
        nextStateChange = Time.time + Random.Range(3f, 6f);
    }

    void OnStateToChasing()
    {
        Debug.Log(gameObject.name + " to Chasing");
        currState = State.Chasing;
        agent.SetDestination(Sinner.instance.transform.position);
        currTargetLoc = Sinner.instance.transform.position;
        nextStateChange = Time.time + Random.Range(3f, 6f);
    }

    void OnStateToGuarding()
    {
        Debug.Log(gameObject.name + " to Guarding");
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
