using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Restart : MonoBehaviour
{
    public static Restart Instance;
    public void RestartGame()
    {
        DOTween.KillAll();
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
    }
}
