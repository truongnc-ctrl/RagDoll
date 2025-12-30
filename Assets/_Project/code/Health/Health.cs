using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject followtransform;
    public static List<Health> ActiveInstances = new List<Health>(); 
    public static event Action<Health> OnHealthAdded;
    public static event Action<Health> OnHealthRemoved;
    public event Action OnDeath;

    public float maxHealth = 100f;
    public float currentHealth;
    public Vibrations vibrations;
    public DamageNumber numberPrefab;
    private bool isDead = false;
    private float TotalDamage = 0f; 
    private Coroutine CaculatorDamage; 

    void OnEnable()
    {
        if (!ActiveInstances.Contains(this)) ActiveInstances.Add(this);
        OnHealthAdded?.Invoke(this);
    }

    void OnDisable()
    {
        if (ActiveInstances.Contains(this)) ActiveInstances.Remove(this);
        OnHealthRemoved?.Invoke(this);
    }

    void Start()
    {
        currentHealth = maxHealth;
        vibrations = GetComponent<Vibrations>();
        if (healthBar != null) healthBar.Initialize(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        Ouch_sound_enemy.Instance.PlayOuchSound(); 
        if(amount > 0)
        {
            TotalDamage += amount;
            if (CaculatorDamage == null)
            {
                CaculatorDamage = StartCoroutine(ShowBatchedDamage());
            }
        }
        if (vibrations != null && Vibration_settings.instance.vibration_on == true)
        {
            vibrations.LightVibration();
        }
        if (currentHealth < 0) currentHealth = 0;
        if (healthBar != null) healthBar.UpdateHealthUI(currentHealth);
        if (currentHealth <= 0) Die();
    }

    private IEnumerator ShowBatchedDamage()
    {
        yield return new WaitForSeconds(0.1f);

        if (TotalDamage > 0)
        {
            DamageNumber damageNumber = numberPrefab.Spawn(followtransform.transform.position + Vector3.up * 5f, TotalDamage,followtransform.transform);
        }

        TotalDamage = 0f;
        CaculatorDamage = null;
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke(); 
    }
}