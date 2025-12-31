using UnityEngine;
using DG.Tweening;

public class PatrolObject : MonoBehaviour
{
    [SerializeField] private float height = 11f; 
    [SerializeField] private float duration = 1f;

    void Start()
    {

        float targetY = transform.position.y + height;
        transform.DOMoveY(targetY, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}