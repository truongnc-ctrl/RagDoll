using UnityEngine;
using UnityEngine.SceneManagement;

public class Sence_Manager : MonoBehaviour
{
    public static Sence_Manager Instance;
    public int Sence_index = 0;
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
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


}
