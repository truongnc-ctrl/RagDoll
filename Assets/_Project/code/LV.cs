using UnityEngine;
using TMPro;

public class LV : MonoBehaviour
{
    [SerializeField] private TMP_Text lv;
    void Start()
    {
        lv.text = Sence_Manager.Instance.Sence_index.ToString();
    }
}
