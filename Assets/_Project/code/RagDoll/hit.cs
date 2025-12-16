using UnityEngine;

public class hit : MonoBehaviour
{
    [Header("Settings")]
    public Rigidbody2D hipsRigidbody;
    public float damageThresholdToRagdoll = 20f; 
    private float minDamageVelocity; 
    private float maxDamageVelocity; 

    private Health _health;
    private RagdollReset ragdollReset; 
    private Collider2D collider_;
    public bool stand;
    public bool iscollision;
    private Line line;

    void Start()
    {
        ragdollReset = GetComponent<RagdollReset>();
        collider_ = GetComponent<Collider2D>(); 
        stand = true;
        _health = GetComponentInChildren<Health>(); 
        line = FindFirstObjectByType<Line>();
        iscollision = false;

        if (line != null)
        {
            maxDamageVelocity = line.maxPower; 
            minDamageVelocity = maxDamageVelocity * 0.1f; 
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        iscollision = true;
        
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("weapon"))
        {   
            WeaponInfo weaponInfo = collision.collider.GetComponent<WeaponInfo>();
            
            if (weaponInfo != null && weaponInfo._weapon != null)
            {
                float baseDamage = weaponInfo._weapon.damage;
                float baseForce = weaponInfo._weapon.knockbackForce;

                float finalDamage = baseDamage;
                float finalForce = baseForce;

                // 1. Giảm dame nếu là Nade (chờ nổ mới gây dame to)
                if (weaponInfo._weapon.currentWeaponType == TypeWeapon.nade)
                {
                    finalDamage = baseDamage * weaponInfo._weapon.DamageCollisionNade; 
                    finalForce = baseForce * weaponInfo._weapon.DamageCollisionNade;
                }

                // 2. Lấy vận tốc va chạm thực tế
                float impactVelocity = collision.relativeVelocity.magnitude;

                // Nếu va chạm quá nhẹ thì bỏ qua
                if (impactVelocity < minDamageVelocity) return;

                // 3. Tính toán hệ số lực (từ 0 đến 1)
                float velocityFactor = Mathf.Clamp01((impactVelocity - minDamageVelocity) / (maxDamageVelocity - minDamageVelocity));

                finalDamage *= velocityFactor;
                finalForce *= velocityFactor;

                Vector2 dir = (transform.position - collision.transform.position).normalized;
                
                if (finalDamage > 0)
                {
                    ReceiveImpact(finalDamage, finalForce, dir);
                }
            }
        }
    }

    public void ReceiveImpact(float damage, float knockback, Vector2 direction)
    {
        if (ragdollReset != null && ragdollReset.IsHoldingLine()) return;

        if (_health != null)
        {
            _health.TakeDamage(damage);
        }
        else
        {
            IDamageable damageable = GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);
        }

        if (damage >= damageThresholdToRagdoll)
        {
            if (ragdollReset != null)
            {
                ragdollReset.TriggerFall(); 
            }
            
            if (stand) 
            {
                stand = false;
                if (collider_ != null) collider_.enabled = false;
            }

            if (hipsRigidbody != null)
            {
                hipsRigidbody.linearVelocity = Vector2.zero; 
                hipsRigidbody.AddForce(direction * knockback, ForceMode2D.Impulse);
            }
        }
    }
}