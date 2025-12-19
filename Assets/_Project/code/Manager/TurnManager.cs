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
    private Queue<MonoBehaviour> turnQueue = new Queue<MonoBehaviour>();
    public bool IsPlayerTurn => currentState == GameState.PlayerTurn;

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
        if (enemy.IsDead) livingEnemies.Remove(enemy);
        Debug.Log(enemy + "remove");
        Debug.Log( livingEnemies.Contains(enemy));
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
                lose.SetActive(true);
                win.SetActive(false);
                StopAllCoroutines();
            }
        }
    }

    public void StartNewRound()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;
        Finish_turn = true;

        currentState = GameState.Processing;
        turnQueue.Clear();

        if (line == null || line.IsDead)
        {
            line = FindFirstObjectByType<Line>();
        }

        string debugOrder = "Thứ tự lượt vòng này: ";
        bool hasEnemies = false;

        foreach (var enemy in livingEnemies)
        {
            if(enemy.IsDead == true)
            {
                Debug.Log("Enemy Die");


            }
            if (enemy != null && !enemy.IsDead)
            {
                hasEnemies = true;
                turnQueue.Enqueue(line);
                debugOrder += "[PLAYER] -> ";
                turnQueue.Enqueue(enemy);
                debugOrder += "[" + enemy.name + "] -> ";
            }
        }
        Debug.Log(debugOrder + " HẾT.");

        if (turnQueue.Count > 0)
        {
            ProcessNextTurn();
        }
        else
        {
            if (hasEnemies == false)
            {
                Debug.Log("Không còn kẻ địch nào để xếp hàng. Player Win?");
            }
            else
            {

                StartNewRound();
            }
        }
    }

    public void ProcessNextTurn()
    {
        if (currentState == GameState.win || currentState == GameState.lose) return;

        if (turnQueue.Count > 0)
        {
            MonoBehaviour unit = turnQueue.Dequeue();
            
            
            if (unit == null) 
            {
                ProcessNextTurn();
                return;
            }

            if (unit is Line) 
            {
                Line p = unit as Line;
                if (!p.IsDead) StartCoroutine(HandlePlayerTurn());
            }
            else if (unit is Enemy_attack) 
            {
                Enemy_attack e = unit as Enemy_attack;
                if (!e.IsDead)
                {
                    StartCoroutine(HandleEnemyTurn(e));
                } 
                if (e.IsDead)
                {
                   unit = null;
                } 
            }
        }
        else
        {
            StartNewRound();
        }
    }

    public void EndTurn()
    {
        ProcessNextTurn();

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
        while(Finish_turn == false || count > 0)
        {
            yield return null;
        }
        currentState = GameState.PlayerTurn;
        Turn_text.text = "Player turn";
        
    }

    IEnumerator HandleEnemyTurn(Enemy_attack enemy)
    {
        while(Finish_turn == false || count > 0)
        {
            yield return null;
        }   
        yield return new WaitForSeconds(2f);
        currentState = GameState.EnemyTurn;
        enemy.StartAttack();
        Turn_text.text = "Enemy turn";
       
    }
    
    public void ProcessNextEnemy() { EndTurn(); }
    public void EndPlayerTurn() { EndTurn(); }

}