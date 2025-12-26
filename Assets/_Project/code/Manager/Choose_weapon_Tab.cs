using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class Choose_weapon_Tab : MonoBehaviour
{
    [SerializeField] private GameObject Weapon_tab;
    public float open;
    public static Choose_weapon_Tab Instance;
    public bool Ischoose = false;

    void Start()
    {
        Instance = this;
        Ischoose = false;
    }
    public void OpenWeaponTab()
    {
        Weapon_tab.SetActive(true);
        Weapon_tab.transform.DOMoveY(open, 0.5f).SetEase(Ease.OutBack);
        Ischoose = true;
    }
    public void CloseWeaponTab()
    {
        Weapon_tab.transform.DOMoveY(-2000, 0.5f).SetEase(Ease.InBack);
        Ischoose = false;
    }

}
