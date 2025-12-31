using UnityEngine;
using TMPro;

public class LV : MonoBehaviour
{
    [SerializeField] private TMP_Text lv;
    void Start()
    {
        if(Sence_Manager.Instance != null) lv.text = Sence_Manager.Instance.Sence_index.ToString();
    }
}
