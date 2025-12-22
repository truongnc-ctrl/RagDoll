using UnityEngine;

public class Close_weapon_tab : MonoBehaviour
{
    public void CloseWeaponTab()
    {
        this.gameObject.SetActive(false);
        Choose_weapon_Tab.Instance.Ischoose = false;
    }

    
}
