using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour
{
    private ProjectileRotation rotator;
    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rotator = GetComponent<ProjectileRotation>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Prepare()
    {
        if (rotator != null) rotator.SetRotation(false);
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
    }

    public void Throw(Vector2 velocity, float colliderDelay)
    {
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
}