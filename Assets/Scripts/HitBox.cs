using UnityEngine;
using UnityEngine.VFX;

public class HitBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damageAmount;
    [SerializeField] private VisualEffect hitVFX;
    public GameObject owner { get; set; }
    void OnTriggerEnter(Collider other)
    {
        hitVFX.Play();
        if (other.TryGetComponent<HP>(out HP health) && other.gameObject != owner)
        {
            health.TakeDamage(damageAmount);
        }
    }
}
