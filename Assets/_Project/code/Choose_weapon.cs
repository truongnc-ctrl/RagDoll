using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choose_weapon : MonoBehaviour
{
    public static Choose_weapon Instance;
    public List<Weapon> weaponList = new List<Weapon>(); 
    public int Index = 0; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetWeaponIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weaponList.Count)
        {
            Index = newIndex;
            Debug.Log($"Đã chọn vũ khí: {weaponList[Index].name_weapon}");
            
        }

    }

    public Weapon GetCurrentWeaponData()
    {
        if (Index >= 0 && Index < weaponList.Count)
        {
            return weaponList[Index];
        }
        
        Debug.LogError("Weapon Index : " + Index);
        if (weaponList.Count > 0) return weaponList[0]; 
        return null;
    }
    public Weapon GetRandomWeapon()
    {
        if (Index >= 0 && Index < weaponList.Count)
        {
            int i = Random.Range(0, weaponList.Count);
            return weaponList[0];
        }
        
        Debug.LogError("Weapon Index : " + Index);
        if (weaponList.Count > 0) return weaponList[0]; 
        return null;
    }
}