using System.Collections;
using UnityEngine;

public class projecTitle : MonoBehaviour
{
    [SerializeField] private float TimeCollier = 0.05f;
    Rigidbody2D rb;
    Collider2D col; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>(); 

        
        StartCoroutine(EnableColliderAfterTime(TimeCollier)); 
        Destroy(this.gameObject,2f);
    }

    void Update()
    {
        
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator EnableColliderAfterTime(float time)
    {
        col.enabled = false; 
        yield return new WaitForSeconds(time); 
        col.enabled = true; 
    }
}