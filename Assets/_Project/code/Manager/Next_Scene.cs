using UnityEngine;
using UnityEngine.SceneManagement;

public class Next_Sence : MonoBehaviour
{
    public void Next_Sences()
    {
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.Save();
        if (Sence_Manager.Instance != null)
        {
            Sence_Manager.Instance.Sence_index = currentLevel;
        }
        SceneManager.LoadSceneAsync(1); 
    }
}