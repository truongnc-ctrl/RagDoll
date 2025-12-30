using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public static Rocket Instance;
    [SerializeField] private SpriteRenderer laucher;
    [SerializeField] private Rigidbody2D laucher_rb;
    private Bomb Explode;
    public bool hit = false;
    Collider2D col;
    Rigidbody2D rb;
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

    void Start()
    {
        Explode = GetComponent<Bomb>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    
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
           if (rb.linearVelocity.sqrMagnitude > 0.1f) 
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
        }
    }
    private IEnumerator DestroyLaucher()
    {
        yield return new WaitForSeconds(2f);
        Destroy(laucher_rb.gameObject);
    }

}
