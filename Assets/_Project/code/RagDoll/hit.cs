using UnityEngine;
using System.Collections.Generic;

public class hit : MonoBehaviour
{
    [System.Serializable]
    public class BoneProfile
    {
        public string namePart;      
        public Transform bone;      
        public float multiplier = 1f; 
    }

    [Header("Body Parts Settings")]
    public List<BoneProfile> bodyParts = new List<BoneProfile>(); 

    [Header("Settings")]
    public Rigidbody2D hipsRigidbody;
    public float damageThresholdToRagdoll = 20f;
    public float minKnockbackForce = 5f;

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
        else
        {
            maxDamageVelocity = 20f;
            minDamageVelocity = 2f;
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

                if (weaponInfo._weapon.currentWeaponType == TypeWeapon.nade)
                {
                    finalDamage = baseDamage * (weaponInfo._weapon is not null ? 0.2f : 1f);
                    finalForce = baseForce * 0.2f;
                }

                // 2. Tính toán lực va chạm
                float impactVelocity = collision.relativeVelocity.magnitude;
                if (impactVelocity < minDamageVelocity) return;

                float velocityFactor = Mathf.Clamp01((impactVelocity - minDamageVelocity) / (maxDamageVelocity - minDamageVelocity));
                finalDamage *= velocityFactor;
                finalForce *= velocityFactor;
                // Lấy điểm va chạm đầu tiên
                Vector2 hitPoint = collision.GetContact(0).point;
                
                // Tìm xương gần nhất
                float partMultiplier = GetMultiplierFromClosestBone(hitPoint);
                
                finalDamage *= partMultiplier; // Nhân hệ số vào dame cuối
                Vector2 dir = (transform.position - collision.transform.position).normalized;
                if (finalDamage > 0)
                {
                    ReceiveImpact(finalDamage, finalForce, dir);
                }
            }
        }
    }

    float GetMultiplierFromClosestBone(Vector2 hitPoint)
    {
        if (bodyParts.Count == 0) return 1f; 
        float closestSqrDistance = float.MaxValue; 
        float selectedMultiplier = 1f;
        string hitPartName = "None"; 

        foreach (var part in bodyParts)
        {
            if (part.bone == null) continue;
            float sqrDist = (hitPoint - (Vector2)part.bone.position).sqrMagnitude;
            if (sqrDist < closestSqrDistance)
            {
                closestSqrDistance = sqrDist;
                selectedMultiplier = part.multiplier;
                hitPartName = part.namePart;
            }
        }

        Debug.Log($"Trúng phần: {hitPartName} (SqrDist: {closestSqrDistance})");
        return selectedMultiplier;
    }

    public void ReceiveImpact(float damage, float knockback, Vector2 direction)
    {
        if (ragdollReset != null && ragdollReset.IsHoldingLine()) return;

        if (_health != null) _health.TakeDamage(damage);
        else
        {
            IDamageable damageable = GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);
        }

        if (damage >= damageThresholdToRagdoll || knockback >= minKnockbackForce)
        {
            if (ragdollReset != null) ragdollReset.TriggerFall(); 
            
            if (stand) 
            {
                stand = false;
                if (collider_ != null) collider_.enabled = false;
            }

            if (hipsRigidbody != null && knockback >= minKnockbackForce)
            {
                hipsRigidbody.linearVelocity = Vector2.zero; 
                hipsRigidbody.AddForce(direction * knockback, ForceMode2D.Impulse);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach(var part in bodyParts)
        {
            if(part.bone != null) Gizmos.DrawWireSphere(part.bone.position, 0.2f);
        }
    }
}