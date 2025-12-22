using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Choose_weapon_Tab : MonoBehaviour
{
    [SerializeField] private GameObject Weapon_tab;
    public static Choose_weapon_Tab Instance;
    public bool Ischoose = false;

    void Start()
    {
        Weapon_tab.SetActive(false);
        Instance = this;
        Ischoose = false;
    }
    public void OpenWeaponTab()
    {
        Weapon_tab.SetActive(true);
        Ischoose = true;
    }
}
