
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

    // Round-robin index: which enemy should act next (1 enemy per player turn)
    private int nextEnemyIndex = 0;
    private Enemy_attack currentEnemy;
    private bool currentEnemyRemovedThisTurn = false;
    private bool isEnemyPhase = false;

    private int count=0;
    public bool hasExploded = false;
    public bool Finish_turn = true;
    public bool hasCollided = false;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        lose.SetActive(false);
        win.SetActive(false);
    }

    void Start()
    {
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
        if (!livingEnemies.Contains(enemy) && enemy.IsDead == false) livingEnemies.Add(enemy);
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
                lose.SetActive(true);
                win.SetActive(false);
                StopAllCoroutines();
            }
        }
        else if ((defeatedLayer & enemyLayer) != 0)
        {
            if (currentState != GameState.lose)
            {
                currentState = GameState.win;
                lose.SetActive(false);
                win.SetActive(true);
                StopAllCoroutines();
            }
        }
    }

    public void StartNewRound()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;
        Finish_turn = true;

        // Start at player turn
        isEnemyPhase = false;
        currentEnemy = null;
        currentEnemyRemovedThisTurn = false;

        currentState = GameState.Processing;

        // Drop any enemies that died before the player's turn starts
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

        // Ensure index is within range after cleanup
        if (nextEnemyIndex >= livingEnemies.Count) nextEnemyIndex = 0;

        StartCoroutine(HandlePlayerTurn());
    }

    public void ProcessNextTurn()
    {
        EndTurn();
    }

    private void CleanupDeadEnemies()
    {
        livingEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead);
    }

    public void EndTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        if (isEnemyPhase)
        {
            ProcessNextEnemy();
        }
        else
        {
            EndPlayerTurn();
        }

    }

    public void process()
    {
        count++;
    }
    public void done()
    {
        count--;
        if(count <0 ) count =0 ;
    }


    IEnumerator HandlePlayerTurn()
    {
        // While waiting for physics/animations to settle, do not stay in PlayerTurn
        currentState = GameState.Processing;
        // Chờ cho đến khi projectile xong (Finish_turn == true) AND ragdoll animation xong (count == 0)
        while(Finish_turn == false || count > 0)
        {
            yield return null;
        }
        hasExploded = false;
        currentState = GameState.PlayerTurn;
        Turn_text.text = "Player turn";
    }

    IEnumerator HandleEnemyTurn(Enemy_attack enemy)
    {
        // Immediately leave PlayerTurn so the player can't act twice
        currentState = GameState.Processing;
        while(Finish_turn == false || count > 0)
        {
            yield return null;
        } 

        // Ensure exactly 1 living enemy acts after the player.
        // If the chosen enemy dies before their turn (or during the pre-attack delay),
        // immediately pick the next living enemy instead of giving the player an extra turn.
        while (true)
        {
            CleanupDeadEnemies();
            if (livingEnemies.Count == 0)
            {
                OnTeamDefeated(enemyLayer);
                yield break;
            }

            // If the passed enemy is dead/null, find the next living one.
            if (enemy == null || enemy.IsDead)
            {
                Enemy_attack next = GetNextLivingEnemy(ref nextEnemyIndex);
                if (next == null)
                {
                    OnTeamDefeated(enemyLayer);
                    yield break;
                }

                enemy = next;
            }

            // Pre-attack delay ("thinking")
            yield return new WaitForSeconds(2f);

            // Enemy might die while waiting; if so, loop and pick another.
            if (enemy == null || enemy.IsDead) continue;

            currentEnemy = enemy;
            currentEnemyRemovedThisTurn = false;

            currentState = GameState.EnemyTurn;
            Turn_text.text = "Enemy turn";
            hasExploded = false;

            enemy.StartAttack();
            yield break;
        }
       
    }
    
    public void ProcessNextEnemy()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        CleanupDeadEnemies();
        if (livingEnemies.Count == 0)
        {
            OnTeamDefeated(enemyLayer);
            return;
        }

        // Advance round-robin pointer to the enemy after the one that just acted.
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

        // After 1 enemy acts -> back to player
        isEnemyPhase = false;
        currentState = GameState.Processing;
        StartCoroutine(HandlePlayerTurn());
    }

    public void EndPlayerTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        // After player acts, re-check living enemies and start enemy phase.
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

        // Immediately leave PlayerTurn so Line can reset hasFired
        currentState = GameState.Processing;

        Enemy_attack enemy = GetNextLivingEnemy(ref nextEnemyIndex);
        if (enemy == null)
        {
            OnTeamDefeated(enemyLayer);
            return;
        }

        StartCoroutine(HandleEnemyTurn(enemy));
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
    void Update()
    {
        Debug.Log("Finish_turn: " + Finish_turn + " | hasCollided: " + hasCollided + " | count: " + count);   
    }

}