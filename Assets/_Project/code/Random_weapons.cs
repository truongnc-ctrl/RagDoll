using System.Collections.Generic;
using UnityEngine;

public class Random_weapons : MonoBehaviour
{
    public List<Info_weapon_Tab> Weapon_Tabs; 

    void Start() 
    {
        GenerateRandomOptions();
    }

    public void GenerateRandomOptions()
    {
        if (Choose_weapon.Instance == null) return;
        List<int> Weapons = new List<int>();
        for (int i = 0; i < Choose_weapon.Instance.weaponList.Count; i++)
        {
            Weapons.Add(i);
        }
        for (int i = 0; i < Weapon_Tabs.Count; i++)
        {
            if (Weapons.Count > 0)
            {
                Weapon_Tabs[i].gameObject.SetActive(true);
                int randomIndex = Random.Range(0, Weapons.Count);
                int weaponID = Weapons[randomIndex];
                Weapons.RemoveAt(randomIndex);
                Weapon_Tabs[i].DisplayWeaponInfo(weaponID);
            }
        }
    }
}