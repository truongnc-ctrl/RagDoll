using UnityEngine;

public class Ammo : MonoBehaviour
{
    Rigidbody2D rb;
    public float LaucherForce;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Shoot()
    {
        rb.simulated = true;
        rb.linearVelocity = transform.right * LaucherForce;

    }
}
