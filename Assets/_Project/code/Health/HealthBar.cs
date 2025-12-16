using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider easerSlider;
    [SerializeField] private float lerpSpeed = 0.05f;

    public void Initialize(float maxHealth)
    {
        if (mainSlider != null)
        {
            mainSlider.maxValue = maxHealth;
            mainSlider.value = maxHealth;
        }

        if (easerSlider != null)
        {
            easerSlider.maxValue = maxHealth;
            easerSlider.value = maxHealth;
        }
    }

    public void UpdateHealthUI(float currentHealth)
    {
        if (mainSlider != null)
        {
            mainSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (mainSlider == null || easerSlider == null) return;

        if (Mathf.Abs(easerSlider.value - mainSlider.value) > 0.01f)
        {
            easerSlider.value = Mathf.Lerp(easerSlider.value, mainSlider.value, lerpSpeed);
        }
    }
}