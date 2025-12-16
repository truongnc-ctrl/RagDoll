using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Animation_throw : MonoBehaviour
{
    Animator animator;
    [SerializeField] Line line;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        line = GetComponentInChildren<Line>();
    }

    // Update is called once per frame
    void Update()
    {
        if(line.hold == true)
        {
            animator.SetBool("hold",true);
        }
        else
        {
            animator.SetBool("hold",false);
        }
    }
}
