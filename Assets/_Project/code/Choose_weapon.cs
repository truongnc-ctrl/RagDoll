using UnityEngine;

public class Choose_weapon : MonoBehaviour
{
    public static Choose_weapon Instance;
    public int Index;


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetWeaponIndex(int weaponIndex)
    {
        Index = weaponIndex;
        Debug.Log("Đã chọn vũ khí số: " + Index);
    }
}