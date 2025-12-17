using UnityEngine;

public class Knife : MonoBehaviour
{
    private WeaponInfo weaponInfo;
    private Rigidbody2D rb;
    private Collider2D col;
    
    [Header("State Settings")]
    public float minDamageToStick = 10f;
    
    [Header("Fixed Rotation")]
    public float fixedStuckAngle = -72f; 

    [Header("Status")]
    public bool isStuck = false;
    public bool isSpinning = false; 

    [Header("Visual Settings")]
    public Transform stabPoint;
    
    ProjectileRotation projectileRotation; 

    void Start()
    {
        weaponInfo = GetComponent<WeaponInfo>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        projectileRotation = GetComponent<ProjectileRotation>();
    }

    void Update()
    {
        if (isStuck && projectileRotation != null)
        {
            projectileRotation.SetRotation(false);
        }
    }

    public void SpawmObject(Transform targetBone, Vector3 hitPoint, float fixedStuckAngle, bool flipX)
    {
        if (isStuck) return; 
        isStuck = true;
        ProjectileBehavior pb = GetComponent<ProjectileBehavior>();
        if (pb != null) pb.StopAllCoroutines();
        if (projectileRotation != null) projectileRotation.SetRotation(false);
        transform.rotation = Quaternion.Euler(0, 0, fixedStuckAngle);

        if (flipX)
        {
            Vector3 currentScale = transform.localScale;
            transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
        }
        transform.position = hitPoint;

        if (stabPoint != null)
        {
            Vector3 offset = stabPoint.position - transform.position;
            transform.position -= offset;
        }

        rb.bodyType = RigidbodyType2D.Kinematic; 
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f; 
        rb.freezeRotation = true; 
        rb.simulated = false; 

        if (col != null) col.enabled = false;
        
        transform.SetParent(targetBone);
    }
}