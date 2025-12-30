using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Restart : MonoBehaviour
{
    public void RestartGame()
    {
        DOTween.KillAll();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        Sence_Manager.Instance.Sence_index = currentSceneIndex;
    }
}
