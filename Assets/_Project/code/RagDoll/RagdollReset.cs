using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollReset : MonoBehaviour
{
    [Header("Settings")]
    public float timeToReset = 2.0f;
    public float standUpDuration = 1.0f; // Thời gian để gồng người đứng dậy từ từ (chống nhảy)

    private class BalanceData
    {
        public Balance script;
        public float originalForce;
    }

    private List<BalanceData> _balanceDataList = new List<BalanceData>();
    private Rigidbody2D[] _allRbs;
    private bool isDead = false;
    private bool isRagdolling = false;

    void Awake()
    {
        _allRbs = GetComponentsInChildren<Rigidbody2D>();
        var balances = GetComponentsInChildren<Balance>();
        foreach (var b in balances)
        {
            _balanceDataList.Add(new BalanceData 
            { 
                script = b, 
                originalForce = b.force 
            });
        }
    }

    public void TriggerFall()
    {
        if (isRagdolling || isDead) return;
        StartCoroutine(FallAndStandRoutine());
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        ToggleBalance(false);
    }

    IEnumerator FallAndStandRoutine()
    {
        isRagdolling = true;

        // 1. NGÃ: Tắt Balance để rơi tự do
        ToggleBalance(false);

        yield return new WaitForSeconds(timeToReset);

        if (!isDead)
        {
            // 2. CHUẨN BỊ ĐỨNG:
            // Quan trọng: Triệt tiêu mọi vận tốc quán tính để nhân vật nằm im trước khi dậy
            foreach (var rb in _allRbs)
            {
                rb.linearVelocity = Vector2.zero; // Unity 6 dùng linearVelocity
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
        }

        isRagdolling = false;
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