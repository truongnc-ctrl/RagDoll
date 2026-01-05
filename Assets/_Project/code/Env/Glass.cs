using UnityEngine;

public class Glass : MonoBehaviour
{
    [SerializeField] private float forceThreshold = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= forceThreshold)
        {
           this.gameObject.SetActive(false);
        }
    }

}
