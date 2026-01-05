using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleScreenController : MonoBehaviour
{
    private const float WIDTH_DEFAULT = 1080f;
    private const float HEIGHT_DEFAULT = 1920f;

    private void Awake()
    {
        ScaleScreen();
    }
#if UNITY_EDITOR
    private void FixedUpdate()
    {
        ScaleScreen();
    }
#endif
    private void ScaleScreen()
    {
        float currentWidth = GetComponent<RectTransform>().rect.width;
        float currentHeight = GetComponent<RectTransform>().rect.height;


        float ratioCurrent = currentHeight / currentWidth;
        float ratioDefault = HEIGHT_DEFAULT / WIDTH_DEFAULT;

        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();

        if (ratioCurrent > ratioDefault) canvasScaler.matchWidthOrHeight = 0f;
        if (ratioCurrent < ratioDefault) canvasScaler.matchWidthOrHeight = 1f;
        if (ratioCurrent == ratioDefault) canvasScaler.matchWidthOrHeight = 0.5f;

    }
}
