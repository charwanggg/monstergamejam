using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Sinner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Sinner instance;
    void Awake()
    {
        instance = this;
    }
    
    [SerializeField] private VisualEffect[] clawvfx;
    private bool darkMagicActive;
    [SerializeField] private Animator leftArmAnim;
    [SerializeField] private Animator rightArmAnim;
    [SerializeField] private Collider clawCollider;
    [SerializeField] private ParticleSystem lightning;
    [SerializeField] private HP hp;
    [SerializeField] private LayerMask ritualLayer;
    public bool canChannel;


    private Coroutine channelCoroutine;

    private float nextClawAttack;
    private float darkMagicEndTime;
    [SerializeField] private float clawCD;
    void Start()
    {
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.enabled = false;
        }
        clawCollider.GetComponent<HitBox>().owner = this.gameObject;
        hp.OnDie += Die;
        hp.OnTakeDamage +=(a) => OnTakeDamage(a);
    }

    void Die()
    {

    }
    
    void OnTakeDamage(int damage)
    {
        HitPause.Instance.HitStop(.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextClawAttack && darkMagicActive)
        {
            nextClawAttack = Time.time + clawCD;
            ClawAttack();
        }
        if (Input.GetMouseButton(1) && channelCoroutine == null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f, ritualLayer);
            foreach (Collider hit in hits)
            {
                RitualSpot ritual = hit.GetComponentInParent<RitualSpot>();
                if (!ritual.isExhausted)
                {
                    leftArmAnim.SetTrigger("Channeling");
                    rightArmAnim.SetTrigger("Channeling");
                    channelCoroutine = StartCoroutine(ChannelingCoroutine());
                    break;
                } else
                {
                    Debug.Log("Ritual Exhausted");
                }
            }
            
        }
        if (Time.time > darkMagicEndTime && darkMagicActive)
        {
            darkMagicActive = false;
            Debug.Log("Dark Magic Ended");
        }
    }

    void ClawAttack()
    {
        leftArmAnim.SetTrigger("Claw");
        StartCoroutine(ClawAttackCoroutine());
    }

    private IEnumerator ChannelingCoroutine()
    {
        float currTime = 0f;
        RitualSpot ritual = null;
        while (currTime < 1.5f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f, ritualLayer);
            canChannel = false;
            foreach (Collider hit in hits)
            {
                ritual = hit.GetComponentInParent<RitualSpot>();
                if (!ritual.isExhausted)
                {
                    canChannel = true;
                    break;
                } else
                {
                    Debug.Log("Ritual Exhausted");
                }
            }
            if (canChannel && Input.GetMouseButton(1))
            {
                yield return new WaitForSeconds(0.1f);
                currTime += 0.1f;
            }
            else
            {
                Debug.Log("Channel Interrupted");
                leftArmAnim.SetTrigger("BackToIdle");
                rightArmAnim.SetTrigger("BackToIdle");
                channelCoroutine = null;
                yield break;
            }
        }
        lightning.Play();
        Debug.Log("Ritual complete");
        ritual.ExhaustRitual();

        channelCoroutine = null;
        darkMagicActive = true;
        darkMagicEndTime = Time.time + 10f;
        yield return null;
    }
    
    private IEnumerator ClawAttackCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.enabled = true;
            vfx.Play();
        }
        clawCollider.enabled = true;
        yield return new WaitForSeconds(0.8f);
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.Stop();
            vfx.enabled = false;
        }
        clawCollider.enabled = false;
    }
}
