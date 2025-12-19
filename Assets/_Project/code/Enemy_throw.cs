using UnityEngine;

public class Enemy_throw : MonoBehaviour
{
    Animator animator;
    Enemy_attack enemyAttack;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAttack = GetComponentInChildren<Enemy_attack>();
    }

    void Update()
    {
        if (enemyAttack == null) return;
        if (enemyAttack.isHoldingProjectile)
        {
            animator.SetBool("hold", true);
        }
        else
        {
            animator.SetBool("hold", false);
        }
    }
}