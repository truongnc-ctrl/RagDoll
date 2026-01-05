using UnityEngine;

public class Fall : MonoBehaviour
{
    [SerializeField] private RagdollReset ragdollReset;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private float forceThreshold = 5f;


    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.relativeVelocity.magnitude >= forceThreshold)
        {
            ragdollReset.TriggerFall();
        }
    }

}