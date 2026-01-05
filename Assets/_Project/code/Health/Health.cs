using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;

public class Health : MonoBehaviour
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
    private float _accumulatedDamage = 0f;
    private Coroutine _damageDisplayCoroutine; 

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
        _accumulatedDamage += amount;
        Ouch_sound_enemy.Instance.PlayOuchSound();
        if (vibrations != null && Vibration_settings.instance.vibration_on == true)
        {
            vibrations.LightVibration();
        }

        if (currentHealth < 0) currentHealth = 0;
        if (healthBar != null) healthBar.UpdateHealthUI(currentHealth);
        if (_damageDisplayCoroutine == null)
        {
            _damageDisplayCoroutine = StartCoroutine(DisplayDamageBatch());
        }

        if (currentHealth <= 0) Die();
    }

    private IEnumerator DisplayDamageBatch()
    {
        yield return new WaitForEndOfFrame();

        if (_accumulatedDamage > 1)
        {
            DamageNumber damageNumber = numberPrefab.Spawn(followtransform.transform.position + Vector3.up * 5f, _accumulatedDamage, followtransform.transform);
        }
        _accumulatedDamage = 0f;
        _damageDisplayCoroutine = null;
    }

    private void Die()
    {
        if (isDead) return; 
        isDead = true;
        OnDeath?.Invoke(); 
    }
}