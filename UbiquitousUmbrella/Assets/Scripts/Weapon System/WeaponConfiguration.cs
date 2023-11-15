using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Config", menuName = "Weapon System/New Weapon Configuration", order = 2)]
public class WeaponConfiguration : ScriptableObject
{
    public LayerMask HitMask;
    public Vector3 weaponSpread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;
}