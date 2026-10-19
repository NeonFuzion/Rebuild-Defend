using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] UnityEvent<int, float> onTakeDamage;
    [SerializeField] UnityEvent onDeath;

    int maxHealth;

    void Awake()
    {
        maxHealth = health;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        onTakeDamage?.Invoke(damage, (float)health / maxHealth);
        health -= damage;

        if (health > 0) return;
        onDeath?.Invoke();
    }
}
