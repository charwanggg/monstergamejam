using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class HitBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damageAmount;
    [SerializeField] private VisualEffect hitVFX;
    public GameObject owner { get; set; }
    void Start()
    {
        this.GetComponent<Collider>().enabled = false;
        hitVFX.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HP>(out HP health) && other.gameObject != owner)
        {
            health.TakeDamage(damageAmount);
            hitVFX.transform.position = other.gameObject.transform.position;
            hitVFX.enabled = true;
            hitVFX.Play();
        }
        turnOffVFX();
    }

    private IEnumerator turnOffVFX()
    {
        yield return new WaitForSeconds(1f);
        hitVFX.Stop();
        hitVFX.enabled = false;
    }
}
