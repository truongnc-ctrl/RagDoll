using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum GameState
{
    Processing, 
    PlayerTurn, 
    EnemyTurn,  
    win,
    lose
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    [Header("UI")]
    [SerializeField] private TMP_Text Turn_text;


    [Header("Settings")]
    public GameState currentState;

    [Header("Layer Config")]
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public Line line; 

    [Header("Panel")]
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject win;

    private List<Enemy_attack> livingEnemies = new List<Enemy_attack>();
    public bool IsPlayerTurn => currentState == GameState.PlayerTurn;
    public bool IsProcess => currentState == GameState.Processing;

    private int nextEnemyIndex = 0;
    private Enemy_attack currentEnemy;
    private bool currentEnemyRemovedThisTurn = false;
    private bool isEnemyPhase = false;

    private Coroutine enemyTurnRoutine;
    private int enemyTurnToken = 0;
    private Coroutine enemyAdvanceAfterResolutionRoutine;


    public bool Finish_turn = true;
    public bool hasCollided = false;
    public bool isRagdolling = false;
    private int ragdollCount = 0;

    public void BeginRagdoll()
    {
        ragdollCount++;
        isRagdolling = ragdollCount > 0;
    }

    public void EndRagdoll()
    {
        ragdollCount = Mathf.Max(0, ragdollCount - 1);
        isRagdolling = ragdollCount > 0;
    }



    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        lose.SetActive(false);
        win.SetActive(false);
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(StartGameRoutine());
    }


    IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        StartNewRound();
    }

    public void SetPlayer(Line player)
    {
        line = player;
    }

    public void RegisterEnemy(Enemy_attack enemy)
    {
        if (!livingEnemies.Contains(enemy) ) livingEnemies.Add(enemy);
        Debug.Log(enemy + "add");
    }

    public void UnregisterEnemy(Enemy_attack enemy)
    {
        if (enemy == null) return;

        int removedIndex = livingEnemies.IndexOf(enemy);
        if (removedIndex < 0) return;

        livingEnemies.RemoveAt(removedIndex);

        // Keep round-robin index stable when list shrinks
        if (livingEnemies.Count > 0)
        {
            if (removedIndex < nextEnemyIndex)
            {
                nextEnemyIndex = Mathf.Max(0, nextEnemyIndex - 1);
            }
            else if (nextEnemyIndex >= livingEnemies.Count)
            {
                nextEnemyIndex = 0;
            }
        }

        if (currentState == GameState.EnemyTurn)
        {
            if (enemy == currentEnemy)
            {
                // Current enemy was removed (died) during its own turn.
                currentEnemyRemovedThisTurn = true;
            }
        }

        Debug.Log(enemy + "remove");

        if (livingEnemies.Count == 0)
        {
            OnTeamDefeated(enemyLayer);
        }
    }

    public void OnTeamDefeated(LayerMask defeatedLayer)
    {
        if ((defeatedLayer & playerLayer) != 0)
        {
            if (currentState != GameState.win)
            {
                currentState = GameState.lose;
                if (lose != null) lose.SetActive(true);
                if (win != null) win.SetActive(false);
                StopAllCoroutines();
            }
        }
        else if ((defeatedLayer & enemyLayer) != 0)
        {
            if (currentState != GameState.lose)
            {
                currentState = GameState.win;
                if (lose != null) lose.SetActive(false);
                if (win != null) win.SetActive(true);
                StopAllCoroutines();
            }
        }
    }

    public void StartNewRound()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;
        Finish_turn = true;
        hasCollided = false;

        if (enemyTurnRoutine != null)
        {
            StopCoroutine(enemyTurnRoutine);
            enemyTurnRoutine = null;
        }
        enemyTurnToken++;

        isEnemyPhase = false;
        currentEnemy = null;
        currentEnemyRemovedThisTurn = false;

        ragdollCount = 0;
        isRagdolling = false;

        currentState = GameState.Processing;

        CleanupDeadEnemies();

        if (line == null || line.IsDead)
        {
            line = FindFirstObjectByType<Line>();
        }

        if (livingEnemies.Count == 0)
        {
            Debug.Log("Không còn kẻ địch nào. Player Win.");
            OnTeamDefeated(enemyLayer);
            return;
        }

        if (nextEnemyIndex >= livingEnemies.Count) nextEnemyIndex = 0;

        StartCoroutine(HandlePlayerTurn());
    }

    public void ProcessNextTurn()
    {
        if(Finish_turn == true && isRagdolling == false)
        {
            EndTurn();
        }
        else
        {
            return;
        }
        
    }

    private void CleanupDeadEnemies()
    {
        livingEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead);
    }

    public void EndTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;
        hasCollided = false;
        Finish_turn = false;

        if (isEnemyPhase)
        {
            ProcessNextEnemy();
        }
        else
        {
            EndPlayerTurn();
        }

    }



    IEnumerator HandlePlayerTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) yield break;
        
        currentState = GameState.Processing;

        Finish_turn = false;
        hasCollided = false;

        currentState = GameState.PlayerTurn;
        Turn_text.text = "Player turn";
        Choose_weapon_Tab.Instance.OpenWeaponTab();
    }

    IEnumerator HandleEnemyTurn(Enemy_attack enemy, int token)
    {
        currentState = GameState.Processing;

        Finish_turn = false;
        hasCollided = false;

        yield return new WaitForSeconds(2f);

        if (token != enemyTurnToken || !isEnemyPhase || currentState == GameState.win || currentState == GameState.lose)
        {
            yield break;
        }

        CleanupDeadEnemies();
        if (livingEnemies.Count == 0)
        {
            OnTeamDefeated(enemyLayer);
            yield break;
        }

        while (enemy == null || enemy.IsDead)
        {
            enemy = GetNextLivingEnemy(ref nextEnemyIndex);
            if (enemy == null)
            {
                OnTeamDefeated(enemyLayer);
                yield break;
            }
        }

        if (token != enemyTurnToken || !isEnemyPhase || currentState == GameState.win || currentState == GameState.lose)
        {
            yield break;
        }

        currentEnemy = enemy;
        currentEnemyRemovedThisTurn = false;
        currentState = GameState.EnemyTurn;
        if (Turn_text != null) Turn_text.text = "Enemy turn";
        enemy.StartAttack();
       
    }
    
    public void ProcessNextEnemy()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        if (enemyTurnRoutine != null)
        {
            StopCoroutine(enemyTurnRoutine);
            enemyTurnRoutine = null;
        }
        enemyTurnToken++;

        CleanupDeadEnemies();
        if (livingEnemies.Count == 0)
        {
            OnTeamDefeated(enemyLayer);
            return;
        }

        if (!currentEnemyRemovedThisTurn && currentEnemy != null)
        {
            int idx = livingEnemies.IndexOf(currentEnemy);
            if (idx >= 0)
            {
                nextEnemyIndex = idx + 1;
            }
        }
        if (livingEnemies.Count > 0) nextEnemyIndex %= livingEnemies.Count;

        currentEnemyRemovedThisTurn = false;
        currentEnemy = null;


        isEnemyPhase = false;
        currentState = GameState.Processing;
        StartCoroutine(HandlePlayerTurn());
    }

    public void RequestProcessNextEnemyAfterResolution()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        if (enemyAdvanceAfterResolutionRoutine != null)
        {
            StopCoroutine(enemyAdvanceAfterResolutionRoutine);
            enemyAdvanceAfterResolutionRoutine = null;
        }

        enemyAdvanceAfterResolutionRoutine = StartCoroutine(ProcessNextEnemyAfterResolutionRoutine());
    }

    private IEnumerator ProcessNextEnemyAfterResolutionRoutine()
    {
        while (currentState != GameState.win
               && currentState != GameState.lose
               && (!Finish_turn || isRagdolling))
        {
            yield return null;
        }

        enemyAdvanceAfterResolutionRoutine = null;

        if (currentState == GameState.win || currentState == GameState.lose) yield break;
        ProcessNextEnemy();
    }

    public void EndPlayerTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        CleanupDeadEnemies();
        if (livingEnemies.Count == 0)
        {
            OnTeamDefeated(enemyLayer);
            return;
        }

        if (nextEnemyIndex >= livingEnemies.Count) nextEnemyIndex = 0;

        isEnemyPhase = true;
        currentEnemy = null;
        currentEnemyRemovedThisTurn = false;
        hasCollided = false;
        Finish_turn = false;

        currentState = GameState.Processing;

        Enemy_attack enemy = GetNextLivingEnemy(ref nextEnemyIndex);
        if (enemy == null)
        {
            OnTeamDefeated(enemyLayer);
            return;
        }

        if (enemyTurnRoutine != null)
        {
            StopCoroutine(enemyTurnRoutine);
            enemyTurnRoutine = null;
        }
        enemyTurnToken++;
        int token = enemyTurnToken;
        enemyTurnRoutine = StartCoroutine(HandleEnemyTurn(enemy, token));
    }

    private Enemy_attack GetNextLivingEnemy(ref int startIndex)
    {
        CleanupDeadEnemies();
        if (livingEnemies.Count == 0) return null;

        if (startIndex < 0) startIndex = 0;
        if (startIndex >= livingEnemies.Count) startIndex = 0;

        int attempts = livingEnemies.Count;
        int idx = startIndex;
        while (attempts-- > 0)
        {
            Enemy_attack candidate = livingEnemies[idx];
            if (candidate != null && !candidate.IsDead)
            {
                startIndex = idx;
                return candidate;
            }
            idx = (idx + 1) % livingEnemies.Count;
        }

        return null;
    }

}