using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Info_weapon_Tab : MonoBehaviour
{
    public Image weaponImage;
    public TMP_Text weaponNameText; 
    private int Index_weapon; 

    public void DisplayWeaponInfo(int Index)
    {
        Index_weapon = Index; 
        if (Choose_weapon.Instance != null && Index < Choose_weapon.Instance.weaponList.Count)
        {
            Weapon data = Choose_weapon.Instance.weaponList[Index];
            if (weaponNameText != null) weaponNameText.text = data.name_weapon;
            if (weaponImage != null) weaponImage.sprite = data.icon;
        }
    }

    public void Onclick()
    {
        if (Choose_weapon.Instance != null)
        {
            Choose_weapon.Instance.Index = Index_weapon;
        }
    }
}