using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float fieldOfImpact = 5f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private ParticleSystem explosionEffect;
    public LayerMask layerMask;

    private WeaponInfo weaponInfo;

    private bool isArmed = false;
    private bool exploded = false;
    private Coroutine fuseRoutine;

    void Start()
    {
        weaponInfo = GetComponent<WeaponInfo>();
    }

    public void Arm()
    {
        if (isArmed || exploded) return;
        isArmed = true;
        if (fuseRoutine != null) StopCoroutine(fuseRoutine);
        fuseRoutine = StartCoroutine(FuseRoutine());
    }

    public void Explode()
    {
        if (exploded || weaponInfo?._weapon == null) return;
        exploded = true;
        if (explosionEffect != null)
        {
            explosionEffect.transform.SetParent(null);
            explosionEffect.Play();
            Destroy(explosionEffect.gameObject, explosionEffect.main.duration);
        }
        Bomb_Sound.Instance?.PlayExplosionSound();
        if (Camera_shake_settings.instance.shake_on) Camera_Shake.Instance?.Shake();
        if (TurnManager.Instance != null) TurnManager.Instance.Finish_turn = true;

        float maxDamage = weaponInfo._weapon.damage;
        float maxKnockback = weaponInfo._weapon.knockbackForce;
        Vector3 explosionPos = transform.position;

        Collider2D[] objects = Physics2D.OverlapCircleAll(explosionPos, fieldOfImpact, layerMask);
        HashSet<Collider2D> processedColliders = new HashSet<Collider2D>();

        foreach (Collider2D obj in objects)
        {
            if (processedColliders.Contains(obj)) continue;
            processedColliders.Add(obj);
            Vector2 direction = obj.transform.position - explosionPos;
            float distance = direction.magnitude;
            // Tính tỉ lệ lực theo khoảng cách (Gần = 1, Xa = 0)
            float proximity = Mathf.Clamp01(1 - (distance / fieldOfImpact));
            float baseDamage = maxDamage * proximity;
            float finalForce = maxKnockback * proximity;
            Vector2 pushDir = direction.normalized;

            if (obj.TryGetComponent<BodyPartHit>(out var partHit) && partHit.mainScript != null)
            {
                float partMultiplier = partHit.damageMultiplier;
                float finalDamage = baseDamage * partMultiplier;
                finalDamage *= 0.5f;
                float forceToApply = Mathf.Max(finalForce, partHit.mainScript.minKnockbackForce);
                partHit.mainScript.ReceiveImpact(finalDamage, forceToApply, pushDir, partHit.transform);
            }
            else if (obj.TryGetComponent<hit>(out var hitScript))
            {
                 float forceToApply = Mathf.Max(finalForce, hitScript.minKnockbackForce);
                 hitScript.ReceiveImpact(baseDamage, forceToApply, pushDir, obj.transform);
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator FuseRoutine()
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