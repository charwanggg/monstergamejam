using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Sinner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private VisualEffect[] clawvfx;
    [SerializeField] private bool darkMagicActive;
    [SerializeField] private Animator leftArmAnim;
    [SerializeField] private Animator rightArmAnim;
    [SerializeField] private Collider clawCollider;

    private float nextClawAttack;
    private float clawCD;
    void Start()
    {
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextClawAttack)
        {
            nextClawAttack = Time.time + clawCD;
            ClawAttack();
        }
    }

    void ClawAttack()
    {
        leftArmAnim.SetTrigger("Claw");
        
        StartCoroutine(ClawAttackCoroutine());
    }
    
    private IEnumerator ClawAttackCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.enabled = true;
            vfx.Play();
        }
        yield return new WaitForSeconds(0.25f);
        clawCollider.enabled = true;
        yield return new WaitForSeconds(0.6f);
        foreach (VisualEffect vfx in clawvfx)
        {
            vfx.Stop();
            vfx.enabled = false;
        }
        clawCollider.enabled = false;
    }
}
