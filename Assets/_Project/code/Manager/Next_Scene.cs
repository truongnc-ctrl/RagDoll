using UnityEngine;
using UnityEngine.SceneManagement;

public class Next_Sence : MonoBehaviour
{
    public void Next_Sences()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        Sence_Manager.Instance.Sence_index +=1;
    }

}
