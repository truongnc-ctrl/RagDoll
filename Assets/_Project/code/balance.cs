using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class balance : MonoBehaviour
{
    [SerializeField]public float targetrotaion;
    [SerializeField] private float force;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    void FixedUpdate()
    {
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation,targetrotaion,force* Time.deltaTime));
    }
}
