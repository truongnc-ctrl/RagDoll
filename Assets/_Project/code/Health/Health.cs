using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private RagdollReset ragdollReset;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (ragdollReset == null)
        {
            ragdollReset = GetComponentInParent<RagdollReset>();
        }

        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
        {
            healthBar.UpdateHealthUI(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        
        if (ragdollReset != null)
        {
            ragdollReset.TriggerDeath();
        }
    }
}