using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RagdollReset : MonoBehaviour
{
    public event Action OnFallStart;      
    public event Action OnStandUpComplete;

    [Header("Settings")]
    public float timeToReset = 1.0f;
    public float standUpDuration = 1.0f;
    [SerializeField] float collapseForce = 5.0f; 

    private class BalanceData
    {
        public Balance script;
        public float originalForce;
    }

    private List<BalanceData> _balanceDataList = new List<BalanceData>();
    private Rigidbody2D[] _allRbs;
    private Health _healthScript; 
    
    public bool isDead  = false;


    void Awake()
    {
        _allRbs = GetComponentsInChildren<Rigidbody2D>();
        var balances = GetComponentsInChildren<Balance>();
        foreach (var b in balances)
        {
            _balanceDataList.Add(new BalanceData { script = b, originalForce = b.force });
        }
        _healthScript = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (_healthScript != null) _healthScript.OnDeath += TriggerDeath;
    }

    void OnDisable()
    {
        if (_healthScript != null) _healthScript.OnDeath -= TriggerDeath;
    }

    public void TriggerFall()
    {
        if (TurnManager.Instance.isRagdolling || isDead) return;
        OnFallStart?.Invoke(); 
        StartCoroutine(FallAndStandRoutine());

    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines(); 
        ToggleBalance(false); 
        ForceCollapse(); 
        OnFallStart?.Invoke();
    }


    private void ForceCollapse()
    {
        foreach (var rb in _allRbs)
        {
            rb.AddForce((Vector2.down*2) * collapseForce, ForceMode2D.Impulse);
        }
    }

    IEnumerator FallAndStandRoutine()
    {
        TurnManager.Instance.isRagdolling = true;
        ToggleBalance(false);
        yield return new WaitForSeconds(timeToReset);

        if (!isDead)
        {
            foreach (var rb in _allRbs)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            ToggleBalance(true, 0f);

            float timer = 0f;
            while (timer < standUpDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / standUpDuration;
                foreach (var data in _balanceDataList)
                {
                    data.script.force = Mathf.Lerp(0f, data.originalForce, progress);
                }
                yield return null;
            }
            
            foreach (var data in _balanceDataList)
            {
                data.script.force = data.originalForce;
            }
            OnStandUpComplete?.Invoke();
        }
        TurnManager.Instance.isRagdolling = false;
    }

    private void ToggleBalance(bool state, float overrideForce = -1f)
    {
        foreach (var data in _balanceDataList)
        {
            if (state)
            {
                if (overrideForce >= 0) data.script.force = overrideForce;
                data.script.enabled = true;
            }
            else
            {
                data.script.enabled = false;
            }
        }
    }
}