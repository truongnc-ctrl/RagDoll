using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class projecTitle : MonoBehaviour
{   
    [SerializeField] float WaitTime= 1f;
    [SerializeField] Collider2D col; 
    [SerializeField] float destroy_time=3f;
    Rigidbody2D rb;
    Line line;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>(); 
        line = FindFirstObjectByType<Line>();
        StartCoroutine(EnableColliderDelay());

        Destroy(this.gameObject, destroy_time);
    }

    IEnumerator EnableColliderDelay()
    {
        col.enabled = false;
        yield return new WaitForSeconds(WaitTime); 
        col.enabled = true; 
    }

    void Update()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }
}