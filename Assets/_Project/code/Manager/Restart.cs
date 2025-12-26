using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Restart : MonoBehaviour
{
    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        Time.timeScale = 1;
    }
}
