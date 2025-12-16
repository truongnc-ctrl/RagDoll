using UnityEngine;

public class Standing : MonoBehaviour
{
    [SerializeField] Collider2D feetCollider; 
    
    public bool isGrounded; 

    void Start()
    {
        if (feetCollider == null)
        {
            feetCollider = GetComponent<Collider2D>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Ground"))
        {
            if (feetCollider.IsTouching(collision))
            {
                isGrounded = true;
                Debug.Log("Chân đang chạm đất");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            if (!feetCollider.IsTouching(collision))
            {
                isGrounded = false;
                Debug.Log("Đã nhảy / Rơi");
            }
        }
    }
}