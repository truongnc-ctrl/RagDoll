using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamHealthBar : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask targetLayer; 
    

    public List<Health> healthList = new List<Health>(); 

    [SerializeField] private HealthBar healthBar; 

    private float maxTotalHealth;
    private float currentTotalHealth;
    
  
    private float _lastFrameHealth = -1f; 
    private bool needsRecalculateMax = false;
    private bool hasReportedDefeat = false;

    void Awake()
    {
        if (healthBar == null) healthBar = GetComponentInParent<HealthBar>();
        if (healthBar == null) healthBar = GetComponent<HealthBar>();
    }

    void OnEnable()
    {
        Health.OnHealthAdded += HandleUnitAdded;
        Health.OnHealthRemoved += HandleUnitRemoved;
    }

    void OnDisable()
    {
        Health.OnHealthAdded -= HandleUnitAdded;
        Health.OnHealthRemoved -= HandleUnitRemoved;
    }

    void Start()
    {
        foreach (var unit in Health.ActiveInstances)
        {
            HandleUnitAdded(unit);
        }

        CalculateMaxHealth();
        CalculateCurrentHealth();

        if (healthBar != null)
        {
            healthBar.Initialize(maxTotalHealth);
            healthBar.UpdateHealthUI(currentTotalHealth);
        }
    }

    void Update()
    {
        if (needsRecalculateMax)
        {
            CalculateMaxHealth();
            if (healthBar != null) healthBar.Initialize(maxTotalHealth);
            
            needsRecalculateMax = false;
        }

        CalculateCurrentHealth();

        if (Mathf.Abs(currentTotalHealth - _lastFrameHealth) > 0.01f)
        {
            if (healthBar != null) healthBar.UpdateHealthUI(currentTotalHealth);
            _lastFrameHealth = currentTotalHealth;
        }

        CheckTeamDefeat();
    }

    void CheckTeamDefeat()
    {
        if (hasReportedDefeat) return;
        if (healthList.Count == 0) return; 

        if (currentTotalHealth <= 0)
        {
            hasReportedDefeat = true;
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTeamDefeated(targetLayer);
            }
        }
    }

    void HandleUnitAdded(Health unit)
    {
        if (IsInLayerMask(unit.gameObject.layer, targetLayer))
        {
            if (!healthList.Contains(unit))
            {
                healthList.Add(unit);
                needsRecalculateMax = true;
                if (currentTotalHealth > 0) hasReportedDefeat = false; 
            }
        }
    }

    void HandleUnitRemoved(Health unit)
    {
        if (healthList.Contains(unit))
        {
            healthList.Remove(unit);
            needsRecalculateMax = true;
        }
    }

    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask & (1 << layer)) != 0;
    }

    void CalculateMaxHealth()
    {
        maxTotalHealth = 0;
        foreach (var unit in healthList)
        {
            if (unit != null) maxTotalHealth += unit.maxHealth; 
        }
    }

    void CalculateCurrentHealth()
    {
        currentTotalHealth = 0;
        foreach (var unit in healthList)
        {
            if (unit != null) currentTotalHealth += unit.currentHealth;
        }
    }
}