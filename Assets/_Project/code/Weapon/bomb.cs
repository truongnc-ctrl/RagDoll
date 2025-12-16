using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fieldOfImpact = 5f;
    [SerializeField] private float waitTime = 2f;
    public LayerMask layerMask;

    private WeaponInfo weaponInfo;
    private bool hasExploded = false;
    private bool hasStartedTimer = false;

    void Start()
    {
        weaponInfo = GetComponent<WeaponInfo>();
        if (weaponInfo == null) Debug.LogError("Thiếu script WeaponInfo trên bom!");
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
        if (hasExploded || weaponInfo == null || weaponInfo._weapon == null) return;
        hasExploded = true;

        float maxDamage = weaponInfo._weapon.damage; 
        float maxKnockback = weaponInfo._weapon.knockbackForce;

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact, layerMask);

        foreach (Collider2D obj in objects)
        {

            Vector2 directionVector = obj.transform.position - transform.position;
            float distance = directionVector.magnitude;
            float proximity = Mathf.Clamp01(1 - (distance / fieldOfImpact));

            float finalDamage = maxDamage * proximity;
            float finalForce = maxKnockback * proximity;
            Vector2 pushDirection = directionVector.normalized;

            hit hitScript = obj.GetComponent<hit>();
            if (hitScript != null)
            {
                hitScript.ReceiveImpact(finalDamage, finalForce, pushDirection);
            }
            else
            {
                IDamageable targetHealth = obj.GetComponent<IDamageable>();
                if (targetHealth != null) targetHealth.TakeDamage(finalDamage);

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.AddForce(pushDirection * finalForce, ForceMode2D.Impulse);
            }
        }
        
        Destroy(gameObject); 
    }

    IEnumerator EnableNade()
    {
        yield return new WaitForSeconds(waitTime);
        Explode();
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfImpact);
    }
}