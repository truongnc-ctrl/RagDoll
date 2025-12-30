using UnityEngine;
using UnityEngine.SceneManagement;

public class Next_Sence : MonoBehaviour
{
    public void Next_Sences()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        Sence_Manager.Instance.Sence_index = currentSceneIndex +1;
        Debug.Log(currentSceneIndex+1);


    }

}
