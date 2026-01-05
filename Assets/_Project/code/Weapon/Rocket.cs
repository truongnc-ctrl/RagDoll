using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public static Rocket Instance;
    [SerializeField] private SpriteRenderer laucher;
    [SerializeField] private Rigidbody2D laucher_rb;
    [SerializeField] private Rigidbody2D Rocker_rb;
    [SerializeField]private Bomb Explode;
    [SerializeField] private Collider2D col;
    public bool hit = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode.Explode();
    }
    void FixedUpdate()
    {
        if(hit == true)
        {
            laucher_rb.simulated =true;
           if (Rocker_rb.linearVelocity.sqrMagnitude > 0.1f) 
            {
                float angle = Mathf.Atan2(Rocker_rb.linearVelocity.y, Rocker_rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
        }
    }
    private IEnumerator DestroyLaucher()
    {
        yield return new WaitForSeconds(1f);
        Destroy(laucher_rb.gameObject);
    }

}
