using UnityEngine;
using System;
using System.Collections.Generic;

public class Health : MonoBehaviour, IDamageable
{
    public static List<Health> ActiveInstances = new List<Health>(); 
    public static event Action<Health> OnHealthAdded;
    public static event Action<Health> OnHealthRemoved;
    public event Action OnDeath;

    public float maxHealth = 100f;
    public float currentHealth;
    
    [SerializeField] private HealthBar healthBar;
    private bool isDead = false;

    void OnEnable()
    {
        if (!ActiveInstances.Contains(this))
        {
            ActiveInstances.Add(this);
        }

        OnHealthAdded?.Invoke(this);
    }

    void OnDisable()
    {
        if (ActiveInstances.Contains(this))
        {
            ActiveInstances.Remove(this);
        }

        OnHealthRemoved?.Invoke(this);
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.Initialize(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        if (healthBar != null) healthBar.UpdateHealthUI(currentHealth);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke(); 
    }
}