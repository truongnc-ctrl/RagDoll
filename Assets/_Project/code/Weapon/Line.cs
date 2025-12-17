using UnityEngine;

public class Line : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ProjectileBehavior prefabAmmo; 
    [SerializeField] Transform spawnPoint;
    [SerializeField] LineRenderer lineRenderer;
    
    [Header("Settings")]
    [SerializeField] float force = 5f; 
    public float maxPower = 15f; 
    
    [Header("Trajectory Settings")]
    [SerializeField] int trajectoryStepCount = 15; 
    [SerializeField] float lineStep = 0.05f; 

    [Header("Throw Settings")]
    [SerializeField] float colliderDelay = 0.2f; 
    public bool isHolding;
    private Vector2 startMousePos, currentMousePos, velocity;
    private ProjectileBehavior currentProjectile;

    void Start()
    {
        if(lineRenderer != null) lineRenderer.positionCount = 0;
        isHolding = false;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnProjectile();
            isHolding = true;
        }
        if (isHolding && currentProjectile != null)
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            velocity = (currentMousePos - startMousePos) * force;
            velocity = Vector2.ClampMagnitude(velocity, maxPower);
            
            DrawTrajectory(); 
            RotateLauncher();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isHolding) 
            {
                ReleaseProjectile();
                ClearLine();
                isHolding = false;
            }
        }
    }

    void SpawnProjectile()
    {
        currentProjectile = Instantiate(prefabAmmo, spawnPoint.position, Quaternion.identity);
        currentProjectile.transform.SetParent(spawnPoint);
        currentProjectile.Prepare();
    }

    void ReleaseProjectile()
    {
        if (currentProjectile == null) return;

        currentProjectile.Throw(velocity, colliderDelay);

        currentProjectile = null;
    }

    private void DrawTrajectory()
    {
        Vector3[] positions = new Vector3[trajectoryStepCount];
        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float t = i * lineStep; 
            // Công thức vật lý: s = ut + 1/2at^2
            Vector3 pos = (Vector2)spawnPoint.position + velocity * t + 0.5f * Physics2D.gravity * t * t;
            positions[i] = pos;
        }
        lineRenderer.positionCount = trajectoryStepCount;
        lineRenderer.SetPositions(positions);
    }

    private void RotateLauncher()
    {

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
}