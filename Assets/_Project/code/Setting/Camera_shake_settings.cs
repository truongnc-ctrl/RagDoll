using UnityEngine;
using UnityEngine.UI;

public class Camera_shake_settings : MonoBehaviour
{
    public static Camera_shake_settings instance;
    public bool shake_on = true;
    public  TMPro.TMP_Text Shake_text;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if(!PlayerPrefs.HasKey("shake_on"))
        {
            PlayerPrefs.SetInt("shake_on", 1);
            Load();
        }
        else
        {
            Load();
        }
        UpdateButtonIcon();
    }

    public void OnButtonPress()
    {
        if (shake_on == false)
        {
            shake_on = true;
            UpdateButtonIcon();
        }
        else
        {
            shake_on = false;
            UpdateButtonIcon();
        }
        Save();
        
    }
    private void UpdateButtonIcon()
    {
        if (shake_on == false)
        {
            Shake_text.text = "off";
        }
        else
        {
            Shake_text.text = "on";
        }
    }
    private void Load()
    {
        shake_on = PlayerPrefs.GetInt("shake_on") == 1;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("shake_on", shake_on ? 1 : 0);
    }
}
