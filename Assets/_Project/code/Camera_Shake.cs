using UnityEngine;
using System.Collections;

public class Camera_Shake : MonoBehaviour
{
    public static Camera_Shake Instance;
    

    [Header("Settings")]
    public float duration = 0.5f;
    public float strength = 0.3f;


    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Shake()
    {
        StartCoroutine(Shake_Coroutine());
    }

    IEnumerator Shake_Coroutine()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 randomPoint = Random.insideUnitCircle * strength;
            transform.localPosition = originalPos + randomPoint;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

}