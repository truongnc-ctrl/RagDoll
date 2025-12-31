using UnityEngine;
using UnityEngine.SceneManagement;

public class Next_Sence : MonoBehaviour
{
    public float MapMax =4;
    public void Next_Sences()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        Sence_Manager.Instance.Sence_index +=1;
        if(Sence_Manager.Instance.Sence_index > MapMax)
        {
            Sence_Manager.Instance.Sence_index =0;
        }
    }

}
