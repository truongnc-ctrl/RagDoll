using UnityEngine;

public class Bomb_Sound : MonoBehaviour
{
    public static Bomb_Sound Instance;
    private AudioSource explosionSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        explosionSound = GetComponent<AudioSource>();
    }

    public void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            explosionSound.Play();
        }
    }
}
