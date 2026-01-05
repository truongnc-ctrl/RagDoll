using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float updateInterval = 0.2f; 

    private float accum = 0; 
    private int frames = 0; 
    private float timeleft; 


    private void Start()
    {
        timeleft = updateInterval;
    }

    private void Update()
    {
        timeleft -= Time.unscaledDeltaTime;
        accum += Time.unscaledDeltaTime;
        frames++;

        if (timeleft <= 0.0)
        {
            float fps = frames / accum;
            text.text = string.Format("FPS: {0:F1}", fps);
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}