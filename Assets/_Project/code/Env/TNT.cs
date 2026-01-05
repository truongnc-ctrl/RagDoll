using UnityEngine;

public class TNT : MonoBehaviour
{
    [SerializeField] private float forceThreshold = 5f;
    [SerializeField]private Bomb bomb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= forceThreshold)
        {
           bomb.Explode();
           Destroy(gameObject);
        }
    }
}
