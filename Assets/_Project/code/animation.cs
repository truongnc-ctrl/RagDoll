using System.Collections;
using UnityEngine;

public class animation : MonoBehaviour
{
    RagDoll _ragDoll;    
    hit _hit; 
   
    void Start()
    {
        _ragDoll = GetComponent<RagDoll>();

        _hit = GetComponent<hit>(); 
    }

    public void StartStandUpRoutine()
    {
        StartCoroutine(WaitAndStandUp());
    }

    IEnumerator WaitAndStandUp()
    {
    
        yield return new WaitForSeconds(1f);    
        
        Debug.Log("Đã hết 1 giây, bắt đầu đứng dậy");
        

      
        if (_hit != null)
        {
            _hit.stand = true; 
        }
        else
        {
            Debug.LogError("Không tìm thấy script 'hit' trên nhân vật!");
        }
    }
}