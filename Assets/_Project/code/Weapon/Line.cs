using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [Header("References")]
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
    public int Index_number;

    private Coroutine waitForProjectileRoutine;

    void Start()
    {
        if (lineRenderer != null) lineRenderer.positionCount = 0;
        isHolding = false;
        hasFired = false;
        ragdollReset = GetComponentInParent<RagdollReset>();
        if (ragdollReset != null)
        {
            ragdollReset.OnStandUpComplete += HandleStandUp;
        }

        health = GetComponentInParent<Health>();
        if (health != null) health.OnDeath += OnDeathHandler;
        
        if (TurnManager.Instance != null) TurnManager.Instance.SetPlayer(this);
    }

    void OnDestroy()
    {
        if (ragdollReset != null)
        {

            ragdollReset.OnStandUpComplete -= HandleStandUp;
        }
        if (health != null) health.OnDeath -= OnDeathHandler;
    }

    void HandleFall()
    {
        CancelShot();

    }

    void HandleStandUp()
    {
    }

    void OnDeathHandler()
    {
        isHolding = false;
        IsDead = true;
        CancelShot();        
        if (TurnManager.Instance != null && TurnManager.Instance.IsPlayerTurn)
        {
            TurnManager.Instance.EndTurn();
        }
    }

    void Update()
    {
        if (TurnManager.Instance == null) return;
        
        if (Choose_weapon_Tab.Instance != null && Choose_weapon_Tab.Instance.Ischoose)
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

        if ( !hasFired && !TurnManager.Instance.isRagdolling)
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
                if (currentProjectile != null)
                {
                    isHolding = true;
                    ClearLine();
                }
            }
        }

        if (isHolding && currentProjectile != null)
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float dragDistance = Vector2.Distance(startMousePos, currentMousePos);

            if (dragDistance >= minDragDistance)
            {
                velocity = (startMousePos - currentMousePos) * force;
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
                if (lineRenderer.positionCount > 0 && currentProjectile != null)
                {
                    ReleaseProjectile();
                    ClearLine();
                    hasFired = true;
                    if (waitForProjectileRoutine != null) StopCoroutine(waitForProjectileRoutine);
                    waitForProjectileRoutine = StartCoroutine(WaitForFinishThenEndTurn());
                }
                else
                {
                    CancelShot();
                }
                isHolding = false;
            }
        }
    }

    private IEnumerator WaitForFinishThenEndTurn()
    {
        if (TurnManager.Instance == null) yield break;

        TurnManager.Instance.currentState = GameState.Processing;

        while (TurnManager.Instance != null
               && TurnManager.Instance.currentState != GameState.win
               && TurnManager.Instance.currentState != GameState.lose
               && (!TurnManager.Instance.Finish_turn || TurnManager.Instance.isRagdolling))
        {
            yield return null;
        }

        if (TurnManager.Instance != null
            && TurnManager.Instance.currentState != GameState.win
            && TurnManager.Instance.currentState != GameState.lose)
        {
            TurnManager.Instance.EndTurn();
        }
    }


    void SpawnProjectile()
    {
        if (Choose_weapon.Instance == null) return;
        TurnManager.Instance.hasCollided = false;


        Weapon weaponData = Choose_weapon.Instance.GetCurrentWeaponData();

        if (weaponData != null && weaponData.prefabWeapon != null)
        {

            GameObject newObj = Instantiate(weaponData.prefabWeapon, spawnPoint.position, Quaternion.identity);
            newObj.transform.SetParent(spawnPoint);
            currentProjectile = newObj.GetComponent<ProjectileBehavior>();

            if (currentProjectile != null)
            {
                currentProjectile.Prepare();
                currentProjectile.gameObject.SetActive(false);
                Debug.Log("Spawned weapon: " + weaponData.name_weapon);
            }
            else
            {
                Debug.LogError("Prefab trong Weapon SO thiếu script ProjectileBehavior!");
                Destroy(newObj);
            }
        }
        else
        {
            Debug.LogError("Weapon Data null hoặc chưa gán Prefab trong SO!");
        }
    }

    void ReleaseProjectile()
    {
        if (currentProjectile == null) return;

        currentProjectile.gameObject.SetActive(true);
        currentProjectile.transform.SetParent(null); 
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
        if (velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
}