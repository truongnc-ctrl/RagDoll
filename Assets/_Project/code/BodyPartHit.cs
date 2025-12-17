using UnityEngine;

public class BodyPartHit : MonoBehaviour
{
    [HideInInspector] public hit mainScript; 
    [HideInInspector] public float damageMultiplier = 1f; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mainScript != null)
        {
            mainScript.OnChildCollision(this, collision);
        }
    }
}