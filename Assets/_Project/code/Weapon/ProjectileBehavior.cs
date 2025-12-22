using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Physics Settings")]
    public float stopVelocityThreshold = 0.05f;
    public float maxLifeTime = 10f;


    private ProjectileRotation rotator;
    private Rigidbody2D rb;
    private Collider2D col;
    public LayerMask layerMask;

    void Awake()
    {
        rotator = GetComponent<ProjectileRotation>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
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
            TurnManager.Instance.hasCollided = true;
        }
    }

    void Update()
    {
        if (TurnManager.Instance.Finish_turn == true) return;
        if (TurnManager.Instance.hasCollided == true)
        {
            float sqrThreshold = stopVelocityThreshold * stopVelocityThreshold;
            if (rb.linearVelocity.sqrMagnitude <= sqrThreshold) 
            {
                    TurnManager.Instance.Finish_turn =true; 
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rotator.isSpinning = false;
                
            }
        }

    }
}