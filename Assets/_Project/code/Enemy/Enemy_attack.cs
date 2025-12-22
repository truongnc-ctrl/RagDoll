using UnityEngine;
using System.Collections;

public class Enemy_attack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ProjectileBehavior prefabAmmo; 
    [SerializeField] Transform spawnPoint;
    [SerializeField] LineRenderer lineRenderer;

    [Header("Enemy Settings")]
    public Transform playerTarget; 
    public float minPower = 2f;    
    public float maxPower = 30f; 
    public LayerMask targetLayer;

    [Header("Throw Settings")]
    [SerializeField] float colliderDelay = 0.1f; 
    [SerializeField] int trajectoryStepCount = 15; 
    [SerializeField] float lineStep = 0.05f; 
    [SerializeField] float aim_timming = 1f;
    [SerializeField] float thinking_time = 0.5f;

    public bool isHoldingProjectile = false; 
    private bool isShooting = false;   
    private bool hasAttackedThisTurn = false; 
    public bool IsDead = false;

    private ProjectileBehavior currentProjectile;
    private Health _healthScript;
    private RagdollReset ragdollReset;


    void Start()
    {
        if(lineRenderer != null) lineRenderer.positionCount = 0;
        
        _healthScript = GetComponentInParent<Health>();
        ragdollReset = GetComponentInParent<RagdollReset>();

        if (_healthScript != null) _healthScript.OnDeath += ForceDeath;
        if (TurnManager.Instance != null) TurnManager.Instance.RegisterEnemy(this);
        
        if (ragdollReset != null)
        {
            ragdollReset.OnFallStart += HandleFall;
            ragdollReset.OnStandUpComplete += HandleStandUp;
        }

        if (playerTarget == null)
        {
             foreach (var unit in Health.ActiveInstances)
             {
                if (IsInLayerMask(unit.gameObject.layer, targetLayer))
                {
                    playerTarget = unit.transform;
                    break; 
                }
             }
        }
    }

    void OnDestroy()
    {
        if (_healthScript != null) _healthScript.OnDeath -= ForceDeath;
        if (TurnManager.Instance != null) TurnManager.Instance.UnregisterEnemy(this);
        if (ragdollReset != null)
        {
            ragdollReset.OnFallStart -= HandleFall;
            ragdollReset.OnStandUpComplete -= HandleStandUp;
        }
    }

    public void ForceDeath()
    {
        if (IsDead) return;
        
        IsDead = true; 
        if(ragdollReset != null) ragdollReset.isDead = true; 
        
        StopAllCoroutines();
        CleanupAttack();

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.UnregisterEnemy(this);
            if (TurnManager.Instance.currentState == GameState.EnemyTurn)
            {
                TurnManager.Instance.ProcessNextEnemy(); 
            }
        }
        Debug.Log(name + " died ");
    }

    public void StartAttack()
    {
        hasAttackedThisTurn = false; 

        if (IsDead || (ragdollReset != null && ragdollReset.isDead)) 
        {
            if (TurnManager.Instance != null) TurnManager.Instance.ProcessNextEnemy();
            return;
        }

        if (ragdollReset.isRagdolling) StartCoroutine(WaitAndAttackRoutine());
        else StartCoroutine(PerformAttackRoutine());
    }
    


    IEnumerator PerformAttackRoutine()
    {
        isShooting = true;
        yield return new WaitForSeconds(thinking_time);
        if (IsDead || ragdollReset.isRagdolling) 
        {
            CleanupAttack();
            if (IsDead && TurnManager.Instance != null) TurnManager.Instance.ProcessNextEnemy();
            yield break; 
        }

        SpawnProjectile(); 
        Vector2 throwVelocity = CalculateSimpleTrajectory();
        DrawTrajectory(throwVelocity);
        
        yield return new WaitForSeconds(aim_timming);
        
        if (IsDead || ragdollReset.isRagdolling) 
        {
            CleanupAttack();
            if (IsDead && TurnManager.Instance != null) TurnManager.Instance.ProcessNextEnemy();
            yield break;
        }

        ReleaseProjectile(throwVelocity);
        ClearLine();
        hasAttackedThisTurn = true; 

        yield return new WaitForSeconds(thinking_time); 
        
        isShooting = false;
        if (!ragdollReset.isRagdolling && !IsDead && TurnManager.Instance != null)
        {
            TurnManager.Instance.ProcessNextEnemy();
        }
    }

    IEnumerator WaitAndAttackRoutine()
    {

        while (ragdollReset.isRagdolling) yield return null; 
        
        if (!IsDead) StartCoroutine(PerformAttackRoutine());
        else if (TurnManager.Instance != null) TurnManager.Instance.ProcessNextEnemy();
    }

    IEnumerator WaitAndSkipRoutine()
    {
        while (ragdollReset.isRagdolling) yield return null;
        if (TurnManager.Instance != null) TurnManager.Instance.ProcessNextEnemy();
    }


    void HandleFall()
    {
        ragdollReset.isRagdolling = true;
        
        if (IsDead || ragdollReset.isDead) 
        {
            StopAllCoroutines();
            CleanupAttack();
            return;
        }

        if (TurnManager.Instance != null && TurnManager.Instance.currentState == GameState.EnemyTurn)
        {
            if (isShooting && !hasAttackedThisTurn)
            {

                StopAllCoroutines();
                CleanupAttack();
                StartCoroutine(WaitAndAttackRoutine()); 
            }
            else if (hasAttackedThisTurn)
            {
                StopAllCoroutines();
                CleanupAttack();
                StartCoroutine(WaitAndSkipRoutine()); 
            }
        }
        else
        {

            StopAllCoroutines();
            CleanupAttack();
        }
    }

    void HandleStandUp()
    {
        ragdollReset.isRagdolling = false;

    }


    void CleanupAttack()
    {
        if (lineRenderer != null) lineRenderer.positionCount = 0;
        if (currentProjectile != null) Destroy(currentProjectile.gameObject);
        isHoldingProjectile = false;
        isShooting = false;
    }

    bool IsInLayerMask(int layer, LayerMask mask) { return (mask & (1 << layer)) != 0; }

    Vector2 CalculateSimpleTrajectory()
    {
        if (playerTarget == null) return Vector2.up * minPower;
        Vector3 startPos = spawnPoint.position;
        Vector3 targetPos = playerTarget.position;
        Vector2 direction = targetPos - startPos;
        float distance = direction.magnitude;
        direction.y += distance * 1f; 
        float power = Mathf.Clamp(distance, minPower, maxPower);
        return direction.normalized * power;
    }

    void SpawnProjectile()
    {
        if (currentProjectile != null) Destroy(currentProjectile.gameObject);
        currentProjectile = Instantiate(prefabAmmo, spawnPoint.position, Quaternion.identity);
        currentProjectile.transform.SetParent(spawnPoint);
        currentProjectile.Prepare();
        isHoldingProjectile = true;
    }

    void ReleaseProjectile(Vector2 velocity)
    {
        isHoldingProjectile = false;
        if (currentProjectile == null) return;
        currentProjectile.Throw(velocity, colliderDelay);
        currentProjectile = null;
    }

    private void DrawTrajectory(Vector2 velocity)
    {
        if (lineRenderer == null) return;
        Vector3[] p = new Vector3[trajectoryStepCount];
        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float t = i * lineStep; 
            p[i] = (Vector2)spawnPoint.position + velocity * t + 0.5f * Physics2D.gravity * t * t;
        }
        lineRenderer.positionCount = trajectoryStepCount;
        lineRenderer.SetPositions(p);
    }
    
    private void ClearLine() { if (lineRenderer != null) lineRenderer.positionCount = 0; }

}