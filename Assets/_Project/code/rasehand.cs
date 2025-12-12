using UnityEngine;
using UnityEngine.InputSystem;
public class rasehand : MonoBehaviour
{
    balance _balance;
    [SerializeField]private float _rotation ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _balance = GetComponent<balance>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            _balance.targetrotaion = _rotation;
        }
        else
        {
            _balance.targetrotaion = 0;
        }
    }
}
