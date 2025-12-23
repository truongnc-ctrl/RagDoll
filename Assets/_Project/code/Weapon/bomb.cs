using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fieldOfImpact = 5f;
    [SerializeField] private float waitTime = 1f;
    public LayerMask layerMask;

    private WeaponInfo weaponInfo;
    
    private bool hasStartedTimer = false;    void Start()
    {
        weaponInfo = GetComponent<WeaponInfo>();
        if (weaponInfo == null) Debug.LogError("Missing WeaponInfo on Bomb");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasStartedTimer)
        {
            StartCoroutine(EnableNade());
            hasStartedTimer = true;
        }
    }

    public void Explode()
    {
        if (TurnManager.Instance.hasExploded || weaponInfo == null || weaponInfo._weapon == null) return;
        TurnManager.Instance.hasExploded = true;
        TurnManager.Instance.Finish_turn = true;

        float maxDamage = weaponInfo._weapon.damage; 
        float maxKnockback = weaponInfo._weapon.knockbackForce;
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact, layerMask);

        HashSet<hit> damagedVictims = new HashSet<hit>();

        foreach (Collider2D obj in objects)
        {
            Vector2 directionVector = obj.transform.position - transform.position;
            float distance = directionVector.magnitude;
            float proximity = Mathf.Clamp01(1 - (distance / fieldOfImpact)); 

            float finalDamage = maxDamage * proximity;
            float finalForce = maxKnockback * proximity;
            Vector2 pushDirection = directionVector.normalized;

            BodyPartHit partHit = obj.GetComponent<BodyPartHit>();
            
            if (partHit != null && partHit.mainScript != null)
            {
                float damageToApply = 0f;
                
                if (!damagedVictims.Contains(partHit.mainScript))
                {
                    damageToApply = finalDamage;
                    damagedVictims.Add(partHit.mainScript);
                }
                partHit.mainScript.ReceiveImpact(damageToApply, finalForce, pushDirection, partHit.transform);
            }
            else
            {
                hit hitScript = obj.GetComponent<hit>();
                if (hitScript != null)
                {
                    if (!damagedVictims.Contains(hitScript))
                    {
                        hitScript.ReceiveImpact(finalDamage, finalForce, pushDirection, obj.transform);
                        damagedVictims.Add(hitScript);
                    }
                }
                else
                {
                    IDamageable targetHealth = obj.GetComponent<IDamageable>();
                    if (targetHealth != null) targetHealth.TakeDamage(finalDamage);
                    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                    if (rb != null) rb.AddForce(pushDirection * finalForce, ForceMode2D.Impulse);
                }
            }
        }

        // Reset hasExploded trước destroy để bom lần sau có thể nổ
        TurnManager.Instance.hasExploded = false;
        Destroy(gameObject);
    }

    IEnumerator EnableNade()
    {
        yield return new WaitForSeconds(waitTime);
        Explode();
        Debug.Log("bom nổ");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfImpact);
    }
}