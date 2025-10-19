using UnityEngine;

public class HP : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int maxHealth = 100;
    public event System.Action<int> OnTakeDamage;
    public event System.Action OnDie;
    public int currHP { get; private set; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currHP -= damage;
        if (currHP <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        OnDie?.Invoke();
    }
}
