using UnityEngine;

public class hit : MonoBehaviour
{
    [Header("Cài đặt")]
    public float knockbackForce = 10f;
    public Rigidbody2D hipsRigidbody;

    RagDoll ragDoll;
    Collider2D collider_;
    public bool stand;

    animation playerAnim; 

    void Start()
    {
        ragDoll = GetComponent<RagDoll>();
        collider_ = GetComponent<Collider2D>();
        stand = true;
        

        playerAnim = GetComponent<animation>(); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("weapon"))
        {
            stand = false;
            
            if (ragDoll != null)
            {
                ragDoll.ToggleRagdoll(true);
            }

         
            if (collider_ != null)
            {
                collider_.enabled = false;
            }

           
            Vector2 direction = collision.relativeVelocity.normalized;
            if (hipsRigidbody != null)
            {
                hipsRigidbody.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            playerAnim.StartStandUpRoutine();
        }
    }
    void Update()
    {


    }
}