using UnityEngine;
using UnityEngine.UI;

public class Info_weapon_Tab : MonoBehaviour
{
    public Image weaponImage;
    public TMPro.TMP_Text weaponNameText;
    public int Index = 1;
    
    void Start()
    {
        DisplayWeaponInfo();
    }
    
    void DisplayWeaponInfo()
    {
        if (Choose_weapon.Instance == null || Index < 0 || Index >= Choose_weapon.Instance.weaponList.Count)
        {
            return;
        }
        Weapon weaponData = Choose_weapon.Instance.weaponList[Index];
        if (weaponNameText != null)
        {
            weaponNameText.text = weaponData.name_weapon;
        }
        if (weaponImage != null && weaponData.icon != null)
        {
            weaponImage.sprite = weaponData.icon;
        }
    }

}
