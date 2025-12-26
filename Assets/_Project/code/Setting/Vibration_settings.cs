using System;
using UnityEngine;
using UnityEngine.UI;

public class Vibration_settings : MonoBehaviour
{
    public static Vibration_settings instance;
    public bool vibration_on = true;
    public  TMPro.TMP_Text vibration_Text;

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
        if(!PlayerPrefs.HasKey("vibration_on"))
        {
            PlayerPrefs.SetInt("vibration_on", 0);
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
        Debug.Log("Vibration button pressed");
        if (vibration_on == false)
        {
            vibration_on = true;
            UpdateButtonIcon();
        }
        else
        {
            vibration_on = false;
            UpdateButtonIcon();
        }
        Save();
        
    }
    private void UpdateButtonIcon()
    {
        if (vibration_on == false)
        {
            vibration_Text.text = "off";
        }
        else
        {
            vibration_Text.text = "on";
        }
    }
    private void Load()
    {
        vibration_on = PlayerPrefs.GetInt("vibration_on") == 1;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("vibration_on", vibration_on ? 1 : 0);
    }

}
