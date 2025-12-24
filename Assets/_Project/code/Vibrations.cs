using UnityEngine;
using Lofelt.NiceVibrations;

public class Vibrations : MonoBehaviour
{
    public Health _health;
    public bool isHit= false;
    void Start()
    {
        _health = GetComponent<Health>();
    }
    public void LightVibration()
    {
        if (_health != null && _health.currentHealth > 0)
        {
            Lofelt.NiceVibrations.HapticPatterns.PlayPreset(Lofelt.NiceVibrations.HapticPatterns.PresetType.LightImpact);
            Debug.Log("Light Vibration Triggered");
        }
    }


}
