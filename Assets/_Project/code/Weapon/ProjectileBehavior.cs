using UnityEngine;
using System.Collections;
using System;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Physics Settings")]
    public float StopVelocityThreshold = 3f;
    public float MaxLifeTime = 5f;
    public bool IsDestroyed = true;

    private ProjectileRotation rotator;
    private Rigidbody2D rb;
    private Collider2D col;
    private Knife knife;
    public LayerMask layerMask;
    private float CollisionTimer = 0f;
    private bool StartedDestroyRoutine = false;
    private WeaponInfo _weaponInfo;

    void Awake()
    {
        rotator = GetComponent<ProjectileRotation>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        _weaponInfo = GetComponent<WeaponInfo>();
        if(GetComponent<Knife>() != null) knife = GetComponent<Knife>();
    }

    public void Prepare()
    {
        rotator.SetRotation(false);
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
    }

    public void Throw(Vector2 velocity, float colliderDelay)
    {
        TurnManager.Instance.Finish_turn = false;
        TurnManager.Instance.hasCollided = false;    
        transform.SetParent(null);
        if (rotator != null) rotator.SetRotation(true);
        
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;

        Bomb bomb = GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.Arm();
        }

        StartCoroutine(EnableColliderRoutine(colliderDelay));
    }

    private IEnumerator EnableColliderRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null) col.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if((layerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            bool isNade = _weaponInfo != null
                          && _weaponInfo._weapon != null
                          && _weaponInfo._weapon.currentWeaponType == TypeWeapon.nade;

            if (!isNade)
            {
                TurnManager.Instance.hasCollided = true;
                CollisionTimer = 0f;
                if(IsDestroyed && !StartedDestroyRoutine)
                {
                    StartedDestroyRoutine = true;
                    StartCoroutine(DestroyAfterTimeRoutine());
                }
            }

        }
    }

    private IEnumerator DestroyAfterTimeRoutine()
    {
        while (CollisionTimer < MaxLifeTime)
        {
            if (knife != null && knife.isStuck)
            {
                TurnManager.Instance.Finish_turn = true;
                yield break;
            }
            yield return null;
            CollisionTimer += Time.deltaTime;
        }
        Destroy(gameObject);
        TurnManager.Instance.Finish_turn = true;
        TurnManager.Instance.hasCollided = false;
    }
    void Update()
    {
        if (TurnManager.Instance.Finish_turn == true) return;
        if (TurnManager.Instance.hasCollided == true)
        {
            bool isNade = _weaponInfo != null
                          && _weaponInfo._weapon != null
                          && _weaponInfo._weapon.currentWeaponType == TypeWeapon.nade;

            float stopThresholdSqr = StopVelocityThreshold * StopVelocityThreshold;
            if (rb.linearVelocity.sqrMagnitude <= stopThresholdSqr && !isNade) 
            {
                TurnManager.Instance.Finish_turn =true; 
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rotator.isSpinning = false;
                rotator.SetRotation(false);

            }
        }
    }
}