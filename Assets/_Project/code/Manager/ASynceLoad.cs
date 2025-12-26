using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ASynceLoad : MonoBehaviour
{
    [Header("Menu Scene")]
    [SerializeField] private GameObject Loading_screen_menu;
    [SerializeField] private GameObject Main_menu_UI;
    [SerializeField] private Slider loading_slider_menu;
    void Start()
    {
        LoadLevelBtn(1);
    }

    public void LoadLevelBtn(int SceneId)
    {
        Main_menu_UI.SetActive(false);
        Loading_screen_menu.SetActive(true);
        loading_slider_menu.value = 0; 
        StartCoroutine(LoadAsyncely_Menu(SceneId));
    }

    IEnumerator LoadAsyncely_Menu(int SceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneId);
        operation.allowSceneActivation = false;
        float targetValue = 0f;
        while (!operation.isDone)
        {
            if (operation.progress < 0.9f)
            {
                targetValue = 0.4f;
            }
            else
            {
                targetValue = 1.0f;
            }

            loading_slider_menu.value = Mathf.MoveTowards(loading_slider_menu.value, targetValue, Time.deltaTime * 1.0f);
            if (operation.progress >= 0.9f && loading_slider_menu.value >= 0.99f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}