using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class RagDoll : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private List<Collider2D> _collider;
    [SerializeField] private List<HingeJoint2D> _joints;
    [SerializeField] private List<Rigidbody2D> _rbs;
    [SerializeField] private List<LimbSolver2D> _solvers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleRagdoll(bool ragdollon)
    {
        _anim.enabled = !ragdollon;
        foreach(var col in _collider)
        {
            col.enabled = ragdollon;
        }
        foreach(var joint  in _joints)
        {
            joint.enabled = ragdollon;
        }
        foreach (var rb in _rbs)
        {
            rb.simulated = ragdollon;
        }
        foreach(var solvers in _solvers)
        {
            solvers.weight = ragdollon ? 0 :1;
        }
    }
}
