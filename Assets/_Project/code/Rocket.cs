using UnityEngine;
using DG.Tweening;

public class Rocket : MonoBehaviour
{
    public static Rocket Instance;
    [SerializeField] private SpriteRenderer laucher;
    [SerializeField] private Rigidbody2D laucher_rb;
    private Bomb Explode;
    public bool hit = false;
    Collider2D col;
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
            
        }
    }
}
