using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string name_weapon;
    public Sprite icon;
    public float damage;
    public TypeWeapon currentWeaponType; 
    public float knockbackForce;
    public float DamageCollisionNade;
    public GameObject prefabWeapon;
}

public enum TypeWeapon
{
    ball,
    knife,
    nade
}