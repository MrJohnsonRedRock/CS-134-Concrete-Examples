using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 0.5f;

    private int currentHealth;
    private float lastDamageTime = -999f;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        if (amount <= 0) return;
        if (Time.time - lastDamageTime < invulnerabilityTime) return;

        lastDamageTime = Time.time;
        currentHealth = currentHealth - amount; 

        Debug.Log($"Took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Healed {amount}. Health: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log("Player died.");
        gameObject.SetActive(false);
    }
}