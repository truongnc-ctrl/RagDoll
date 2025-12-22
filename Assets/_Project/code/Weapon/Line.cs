using System;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<ProjectileBehavior> projectilePrefabs = new List<ProjectileBehavior>(); 
    [SerializeField] Transform spawnPoint;
    [SerializeField] LineRenderer lineRenderer;

    
    [Header("Settings")]
    [SerializeField] float force = 5f; 
    public float maxPower = 15f;
    [SerializeField] float minDragDistance = 0.5f;
    
    [Header("Trajectory Settings")]
    [SerializeField] int trajectoryStepCount = 15; 
    [SerializeField] float lineStep = 0.05f; 

    [Header("Throw Settings")]
    [SerializeField] float colliderDelay = 0.2f; 
    public bool isHolding;

    private Vector2 startMousePos, currentMousePos, velocity;
    private ProjectileBehavior currentProjectile;
    private Health health;
    private bool hasFired = false;
    public bool IsDead;
    private RagdollReset ragdollReset; 



    void Start()
    {
        if(lineRenderer != null) lineRenderer.positionCount = 0;
        isHolding = false;
        hasFired = false; 
        ragdollReset = GetComponentInParent<RagdollReset>();
        if (ragdollReset != null)
        {
            ragdollReset.OnFallStart += HandleFall;        
            ragdollReset.OnStandUpComplete += HandleStandUp; 
        }
        
        health = GetComponentInParent<Health>();
        if (health != null)
        {
            health.OnDeath += OnDeathHandler;
        }
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.SetPlayer(this);
        }


    }

    void OnDestroy()
    {
        if (ragdollReset != null)
        {
            ragdollReset.OnFallStart -= HandleFall;
            ragdollReset.OnStandUpComplete -= HandleStandUp;
        }

        if (health != null)
        {
            health.OnDeath -= OnDeathHandler;
        }
    }

    void HandleFall()
    {
        ragdollReset.isRagdolling = true;
        CancelShot();
         if (TurnManager.Instance != null) TurnManager.Instance.process();
    }

    void HandleStandUp()
    {
        ragdollReset.isRagdolling = false;
        if (TurnManager.Instance != null) TurnManager.Instance.done();
    }

    void OnDeathHandler()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.done();
        ragdollReset.isDead = true;
        isHolding = false;
        IsDead=true;
        ClearLine();
        
        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
        }
        if (TurnManager.Instance != null && TurnManager.Instance.IsPlayerTurn)
        {
            TurnManager.Instance.EndTurn();
        }
    }

    void Update()
    {
        if (TurnManager.Instance == null) return;
        if (Choose_weapon_Tab.Instance.Ischoose == true)
        {
            Choose_weapon_Tab.Instance.OpenWeaponTab();
            return;
        }
        
       
        if (!TurnManager.Instance.IsPlayerTurn)
        {
            hasFired = false; 
            if (isHolding) CancelShot(); 
            return;
        }
        if (!ragdollReset.isDead && !hasFired && !ragdollReset.isRagdolling)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentProjectile == null) 
            {
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SpawnProjectile();
                isHolding = true;
                ClearLine(); 
            }
        }
        
        if (isHolding && currentProjectile != null)
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float dragDistance = Vector2.Distance(startMousePos, currentMousePos);

            if (dragDistance >= minDragDistance)
            {
                velocity = (currentMousePos - startMousePos) * force;
                velocity = Vector2.ClampMagnitude(velocity, maxPower);
                
                DrawTrajectory(); 
                RotateLauncher();
            }
            else
            {
                ClearLine(); 
                velocity = Vector2.zero;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isHolding) 
            {
                if (lineRenderer.positionCount > 0)
                {
                    ReleaseProjectile();
                    ClearLine();
                    
                    hasFired = true;
                    TurnManager.Instance.EndTurn();
                }
                else
                {
                    CancelShot();
                }
                isHolding = false;
            }
        }
    }

void SpawnProjectile()
    {
        if (currentProjectile != null) return;
        int weaponIndex = Choose_weapon.Instance.Index;
        if (weaponIndex < 0 || weaponIndex >= projectilePrefabs.Count)
        {
            Debug.LogError($"Lỗi: Index vũ khí ({weaponIndex}) không hợp lệ! Hãy kiểm tra lại List Projectile Prefabs.");
            weaponIndex = 0; 
            if (projectilePrefabs.Count == 0) return; 
        }
        currentProjectile = Instantiate(projectilePrefabs[weaponIndex], spawnPoint.position, Quaternion.identity);
        currentProjectile.transform.SetParent(spawnPoint);
        currentProjectile.Prepare();
        currentProjectile.gameObject.SetActive(false); 
    }

    void ReleaseProjectile()
    {
        if (currentProjectile == null) return;
        currentProjectile.gameObject.SetActive(true);
        currentProjectile.Throw(velocity, colliderDelay);
        currentProjectile = null;
    }

    void CancelShot()
    {
        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
            currentProjectile = null;
        }
        ClearLine();
        isHolding = false;
    }

    private void DrawTrajectory()
    {
        if(currentProjectile != null) currentProjectile.gameObject.SetActive(true);
        Vector3[] positions = new Vector3[trajectoryStepCount];
        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float t = i * lineStep; 
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