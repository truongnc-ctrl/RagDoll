using UnityEngine;
using System.Collections;

public class ProjectileRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 720f; 
    [SerializeField] bool clockwise = true;

    public bool isSpinning = false;


    void Update()
    {
        if (isSpinning)
        {
            float direction = clockwise ? -1f : 1f;
            transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
        }
        StartCoroutine(StopRotation());
        
    }
    public void SetRotation(bool active)
    {
        isSpinning = active;
    }
    private IEnumerator StopRotation()
    {
        yield return new WaitForSeconds(3f);
        isSpinning = false;
    }
}