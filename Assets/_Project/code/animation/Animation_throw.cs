using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Animation_throw : MonoBehaviour
{
    Animator animator;
    Line line;
    void Start()
    {
        animator = GetComponent<Animator>();
        line = GetComponentInChildren<Line>();
    }
    void Update()
    {
        if(line.isHolding == true)
        {
            animator.SetBool("hold",true);
        }
        else
        {
            animator.SetBool("hold",false);
        }
    }
}
