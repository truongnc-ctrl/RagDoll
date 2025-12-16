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
    
    private bool _isRagdoll = false;
    
    void Start()
    {
        RecoverFromRagdoll();
    }

    void Update()
    {
        
    }
    
    public void EnableRagdoll()
    {
        if (_isRagdoll) return;
        
        _isRagdoll = true;
        _anim.enabled = false;
        
        foreach(var col in _collider)
        {
            col.enabled = true;
        }
        foreach(var joint in _joints)
        {
            joint.enabled = true;
        }
        foreach (var rb in _rbs)
        {
            rb.simulated = true;
        }
        foreach(var solver in _solvers)
        {
            solver.weight = 0;
        }
    }
    
    public void RecoverFromRagdoll()
    {
        if (!_isRagdoll && _anim.enabled) return;
        
        _isRagdoll = false;
        _anim.enabled = true;
        
        foreach(var col in _collider)
        {
            col.enabled = false;
        }
        foreach(var joint in _joints)
        {
            joint.enabled = false;
        }
        foreach (var rb in _rbs)
        {
            rb.simulated = false;
        }
        foreach(var solver in _solvers)
        {
            solver.weight = 1;
        }
    }
}