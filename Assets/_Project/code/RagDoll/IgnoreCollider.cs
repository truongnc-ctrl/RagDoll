using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollider : MonoBehaviour
{
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        var colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            for(int k = i + 1; k < colliders.Length; k++)
            {
                Physics2D.IgnoreCollision(colliders[i], colliders[k]);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if((layerMask.value & (1 << coll.gameObject.layer)) > 0)
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), coll.gameObject.GetComponent<Collider2D>());
        }
    }
}