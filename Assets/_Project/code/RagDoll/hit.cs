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
    private Rigidbody2D mainRb; 
    
    public bool stand = true; 
    private Line line;

    void Start()
    {
        ragdollReset = GetComponent<RagdollReset>();
        mainRb = GetComponent<Rigidbody2D>(); 
        _health = GetComponentInChildren<Health>();
        line = FindFirstObjectByType<Line>();
        
        stand = true;

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
        SetupBodyParts();
    }

    void SetupBodyParts()
    {
        foreach (var part in bodyParts)
        {
            if (part.bone != null)
            {
                if (part.bone.GetComponent<Collider2D>() == null)
                {
                    Debug.LogWarning($"Cảnh báo: Xương {part.namePart} chưa có Collider2D! Nó sẽ không nhận được va chạm.");
                }
                BodyPartHit partHit = part.bone.GetComponent<BodyPartHit>();
                if (partHit == null)
                {
                    partHit = part.bone.gameObject.AddComponent<BodyPartHit>();
                }
                partHit.mainScript = this;
                partHit.damageMultiplier = part.multiplier;
            }
        }
    }

    public void SetStand(bool state)
    {
        stand = state;
    }

    public void OnChildCollision(BodyPartHit partHit, Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("weapon")) return;
        if (mainRb != null && stand) 
        {
            mainRb.linearVelocity = Vector2.zero; 
        }

        WeaponInfo weaponInfo = collision.collider.GetComponent<WeaponInfo>();
        Knife incomingKnife = collision.collider.GetComponent<Knife>();
        ProjectileRotation projectileRotation = collision.collider.GetComponent<ProjectileRotation>();
        
        if (projectileRotation != null) projectileRotation.SetRotation(false);
        if (incomingKnife != null && incomingKnife.isStuck) return;

        if (weaponInfo != null && weaponInfo._weapon != null)
        {
            float baseDamage = weaponInfo._weapon.damage;
            float baseForce = weaponInfo._weapon.knockbackForce;
            
            if (weaponInfo._weapon.currentWeaponType == TypeWeapon.nade)
            {
                baseDamage *= 0.2f;
                baseForce *= 0.2f;
            }

            float impactVelocity = collision.relativeVelocity.magnitude;
            if (impactVelocity < minDamageVelocity && incomingKnife == null) return;

            float velocityFactor = Mathf.Clamp01((impactVelocity - minDamageVelocity) / (maxDamageVelocity - minDamageVelocity));
            float finalDamage = baseDamage * velocityFactor;
            float finalForce = baseForce * velocityFactor;

            finalDamage *= partHit.damageMultiplier;
            
            Vector2 knockbackDir = (partHit.transform.position - collision.transform.position).normalized;

            if (finalDamage > 0)
            {
                ReceiveImpact(finalDamage, finalForce, knockbackDir, partHit.transform);
            }

            if (incomingKnife != null && weaponInfo._weapon.currentWeaponType == TypeWeapon.knife)
            {
                HandleKnifeStick(incomingKnife, finalDamage, partHit.transform, collision);
            }
        }
    }

    private void HandleKnifeStick(Knife incomingKnife, float finalDamage, Transform hitBoneTransform, Collision2D collision)
    {
         if (finalDamage >= incomingKnife.minDamageToStick)
         {
             Vector2 contactPoint = collision.GetContact(0).point;
             Transform targetParent = (hitBoneTransform != null) ? hitBoneTransform : transform;

             bool isPlayer = (gameObject.layer == LayerMask.NameToLayer("Player"));
             float stickAngle = incomingKnife.fixedStuckAngle;
             bool shouldFlipX = false; 

             if (isPlayer)
             {
                 stickAngle = -incomingKnife.fixedStuckAngle;
                 shouldFlipX = true; 
             }

             incomingKnife.SpawmObject(targetParent, contactPoint, stickAngle, shouldFlipX);
         }
    }


    public void ReceiveImpact(float damage, float knockback, Vector2 direction, Transform hitBone)
    {

        if (_health != null) 
        {
            _health.TakeDamage(damage);
        }
        else
        {
            IDamageable damageable = GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);
        }

        if (damage >= damageThresholdToRagdoll || knockback >= minKnockbackForce)
        {
            stand = false; 

            if (ragdollReset != null && !ragdollReset.isDead)
            {
                ragdollReset.TriggerFall(); 
            }

            Rigidbody2D targetRb = null;
            if (hitBone != null) targetRb = hitBone.GetComponent<Rigidbody2D>();
            
            if (targetRb == null) targetRb = hipsRigidbody;

            if (targetRb != null)
            {
                targetRb.linearVelocity = Vector2.zero; 
                targetRb.AddForce(direction * knockback, ForceMode2D.Impulse);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var part in bodyParts)
        {
            if (part.bone != null) Gizmos.DrawWireSphere(part.bone.position, 0.2f);
        }
    }
}