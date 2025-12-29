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

    public Weapon GetCurrentWeaponData()
    {
        if (Index >= 0 && Index < weaponList.Count)
        {
            return weaponList[Index];
        }
        if (weaponList.Count > 0) return weaponList[0]; 
        return null;
    }
    public Weapon GetRandomWeapon()
    {
        if (Index >= 0 && Index < weaponList.Count)
        {
            int i = Random.Range(0, weaponList.Count);
            return weaponList[i];
        }
        if (weaponList.Count > 0) return weaponList[0]; 
        return null;
    }
    public List<int> RandomWeaponPlayer(int count)
    {
        List<int> result = new List<int>();

        if (count >= weaponList.Count) 
        {
            for(int i = 0; i < weaponList.Count; i++) result.Add(i);
            return result;
        }
        while (result.Count < count)
        {
            int r = Random.Range(0, weaponList.Count);
            if (!result.Contains(r)) 
            {
                result.Add(r);
            }
        }

        return result;
    }
}