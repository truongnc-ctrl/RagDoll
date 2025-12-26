using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour
{
    [SerializeField] private GameObject settting_panel;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject Main_Game;
    public bool settingIsOpen = false;

    public void open()
    {
        settting_panel.SetActive(true);
        button.SetActive(false);
        Debug.Log("mỏ setting");
        settingIsOpen = true;
        Main_Game.SetActive(false);
    }
    public void close()
    {
        settting_panel.SetActive(false);
        button.SetActive(true);
        Debug.Log("đóng setting");
        settingIsOpen = false;
        if(Main_Game != null) Main_Game.SetActive(true);
    }

}

