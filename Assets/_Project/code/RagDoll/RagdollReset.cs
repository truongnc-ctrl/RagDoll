using UnityEngine;
using System.Collections;

public class RagdollReset : MonoBehaviour
{
    [Header("Setup")]
    public Transform hipsBone;
    public float timeToReset = 2.0f;

    private struct BoneTransform
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    private Transform[] allBones;
    private BoneTransform[] initialBoneTransforms;
    private Rigidbody2D[] ragdollRigidbodies;
    private Collider2D[] ragdollColliders;
    private Rigidbody2D mainRb;
    private Collider2D mainCollider;
    
    private bool isRagdolling = false;
    private bool isDead = false; 
    private Line _line;
    private hit _hitScript;

    void Awake()
    {
        mainRb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        _hitScript = GetComponent<hit>();
        
        allBones = GetComponentsInChildren<Transform>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        ragdollColliders = GetComponentsInChildren<Collider2D>();
        _line = FindFirstObjectByType<Line>();

        initialBoneTransforms = new BoneTransform[allBones.Length];
        for (int i = 0; i < allBones.Length; i++)
        {
            initialBoneTransforms[i] = new BoneTransform
            {
                localPosition = allBones[i].localPosition,
                localRotation = allBones[i].localRotation
            };
        }

        SetRagdollState(false);
    }

    public bool IsHoldingLine()
    {
        return _line != null && _line.hold;
    }
 
    public void TriggerFall()
    {
        if (isRagdolling || isDead) return; 
        StartCoroutine(FallAndStandUpRoutine());
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true; 
        isRagdolling = true;
        StopAllCoroutines(); 
        SetRagdollState(true);
    }

    IEnumerator FallAndStandUpRoutine()
    {
        isRagdolling = true;
        SetRagdollState(true);

        yield return new WaitForSeconds(timeToReset);

        if (isDead) yield break;

        Vector3 ragdollHipsPosition = hipsBone.position;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        transform.rotation = Quaternion.identity;

        for (int i = 0; i < allBones.Length; i++)
        {
            if (allBones[i] != transform)
            {
                allBones[i].localPosition = initialBoneTransforms[i].localPosition;
                allBones[i].localRotation = initialBoneTransforms[i].localRotation;
            }
        }

        Vector3 resetHipsPosition = hipsBone.position;
        Vector3 shiftAmount = ragdollHipsPosition - resetHipsPosition;
        transform.position += shiftAmount;

        SetRagdollState(false);
        
        if (mainRb != null)
        {
            mainRb.linearVelocity = Vector2.zero;
            mainRb.angularVelocity = 0f;
        }
        
        if (_hitScript != null)
        {
            _hitScript.stand = true;
        }

        isRagdolling = false;
    }

    public void SetRagdollState(bool state)
    {
        if (mainRb != null) 
        {
            mainRb.simulated = !state; 
            if (!state) mainRb.linearVelocity = Vector2.zero; 
        }

        if (mainCollider != null) mainCollider.enabled = !state;

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.bodyType = state ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic; 
                if (!state)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
        }

        foreach (var col in ragdollColliders)
        {
            if (col.gameObject != gameObject) col.enabled = state;
        }
    }
}