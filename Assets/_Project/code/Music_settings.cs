using UnityEngine;
using UnityEngine.UI;

public class Music_settings : MonoBehaviour
{
    public static Music_settings instance;
    public GameObject music_on;
    public  Image music_image_on;
    public Image music_image_off;
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
        music_on.SetActive(PlayerPrefs.GetInt("music_on") == 1);
        UpdateButtonIcon();
    }

    public void OnButtonPress()
    {
        Debug.Log("Vibration button pressed");
        if (music_on.activeSelf == false)
        {
            music_on.SetActive(true);
            UpdateButtonIcon();
        }
        else
        {
            music_on.SetActive(false);
            UpdateButtonIcon();
        }
        Save();
        
    }
    private void UpdateButtonIcon()
    {
        if (music_on.activeSelf == false)
        {
            music_image_on.enabled = false;
            music_image_off.enabled = true;
        }
        else
        {
            music_image_on.enabled = true;
            music_image_off.enabled = false;
        }
    }
    private void Load()
    {
        music_on.SetActive(PlayerPrefs.GetInt("music_on") == 1);
    }
    private void Save()
    {
        PlayerPrefs.SetInt("music_on", music_on.activeSelf ? 1 : 0);
    }
}
