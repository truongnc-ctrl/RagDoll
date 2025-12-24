using UnityEngine;

public class Ouch_sound_enemy : MonoBehaviour
{
    public static Ouch_sound_enemy Instance;
    public AudioSource OuchSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void PlayOuchSound()
    {
        if (OuchSound != null && OuchSound.isPlaying == false)
        {
            OuchSound.Play();
        }
    }
}
