using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour
{
    [SerializeField] private GameObject settting_panel;
    [SerializeField] private GameObject button;
    public bool settingIsOpen = false;

    public void open()
    {
        settting_panel.SetActive(true);
        button.SetActive(false);
        Debug.Log("mỏ setting");
        settingIsOpen = true;
        Time.timeScale = 0;
    }
    public void close()
    {
        settting_panel.SetActive(false);
        button.SetActive(true);
        Debug.Log("đóng setting");
        settingIsOpen = false;
        Time.timeScale = 1;
    }

}

