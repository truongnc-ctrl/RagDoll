using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] Transform PrefabAmmo;
    [SerializeField] Transform spawmpoint;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float force = 5f; 
    [SerializeField] float linestep = 0.05f; 
    [SerializeField] int trajectoryStepcount = 5; 
    
    Vector2 velocity, startMousePos, currentMousePos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            velocity = (currentMousePos - startMousePos) * force;
    
            drawTrajectory();
            rotate(); 
        }

        if (Input.GetMouseButtonUp(0))
        {
            FindProjecttilte();
            clear();
        }
    }

    private void drawTrajectory()
    {
        Vector3[] positions = new Vector3[trajectoryStepcount];
        for (int i = 0; i < trajectoryStepcount; i++)
        {
          
            float t = i * linestep; 
            
    
            Vector3 pos = (Vector2)spawmpoint.position + velocity * t + 0.5f * Physics2D.gravity * t * t;
            positions[i] = pos;
        }
        _lineRenderer.positionCount = trajectoryStepcount;
        _lineRenderer.SetPositions(positions);
    }

    private void rotate()
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FindProjecttilte()
    {
        Transform pr = Instantiate(PrefabAmmo, spawmpoint.position, Quaternion.identity);
        Rigidbody2D rb = pr.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = velocity; 
        }
        else
        {
            Debug.LogError("PrefabAmmo thiếu Rigidbody2D!");
        }
    }

    private void clear()
    {
        _lineRenderer.positionCount = 0;
    }
}