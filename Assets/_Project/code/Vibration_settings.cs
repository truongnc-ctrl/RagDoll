using System;
using UnityEngine;
using UnityEngine.UI;

public class Vibration_settings : MonoBehaviour
{
    public static Vibration_settings instance;
    public bool vibration_on = true;
    public  Image vibration_image_on;
    public Image vibration_image_off;
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
            vibration_image_on.enabled = false;
            vibration_image_off.enabled = true;
        }
        else
        {
            vibration_image_on.enabled = true;
            vibration_image_off.enabled = false;
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
