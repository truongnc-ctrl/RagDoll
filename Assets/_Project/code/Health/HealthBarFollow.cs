using UnityEngine;

public class HealthBarPositioner : MonoBehaviour
{
    public Transform targetTransform; 
    public Vector3 offset = new Vector3(0, 0, 0);
    private Quaternion fixedRotation;

    void Start()
    {
        fixedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = targetTransform.position + offset;
        transform.rotation = fixedRotation;
    }
}