using UnityEngine;

public class EnemyFacePlayer : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] bool spriteFaceRightByDefault = true; 

    public void FaceDirection(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.1f) return;
        Vector3 scale = transform.localScale;
        if (directionX > 0) 
        {
            scale.x = spriteFaceRightByDefault ? 1 : -1;
        }
        else 
        {
            scale.x = spriteFaceRightByDefault ? -1 : 1;
        }

        float absScaleX = Mathf.Abs(scale.x);
        scale.x = (scale.x > 0) ? absScaleX : -absScaleX;

        transform.localScale = scale;
    }
}