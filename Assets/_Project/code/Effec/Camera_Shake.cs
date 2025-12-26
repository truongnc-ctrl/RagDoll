using UnityEngine;
using System.Collections;
using DG.Tweening;

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
        Camera.main.transform.DOShakePosition(0.5f, 1f);
    }



}