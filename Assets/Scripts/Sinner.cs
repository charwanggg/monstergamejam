using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Sinner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private VisualEffect[] clawvfx;
    private bool darkMagicActive;
    private bool canDarkMagic;
    [SerializeField] private Animator leftArmAnim;
    [SerializeField] private Animator rightArmAnim;
    [SerializeField] private Collider clawCollider;
    public bool canChannel;

    private float nextClawAttack;
    [SerializeField] private float clawCD;
    void Start()
    {
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.enabled = false;
        }
        clawCollider.GetComponent<HitBox>().owner = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextClawAttack)
        {
            nextClawAttack = Time.time + clawCD;
            ClawAttack();
        }
        if (Input.GetMouseButton(1) && canChannel)
        {
            StartCoroutine(ChannelingCoroutine());
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
        leftArmAnim.SetBool("Channeling", true);
        rightArmAnim.SetBool("Channeling", true);
        while (currTime < 3f)
        {
            if (canChannel && Input.GetMouseButton(1))
            {
                yield return new WaitForSeconds(0.1f);
                currTime += 0.1f;
            }
            else
            {
                leftArmAnim.SetBool("Channeling", false);
                rightArmAnim.SetBool("Channeling", false);
                yield break;
            }
        }
        
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
