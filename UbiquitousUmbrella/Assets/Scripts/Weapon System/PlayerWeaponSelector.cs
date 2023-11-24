using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerWeaponSelector : MonoBehaviour
{
    [SerializeField]
    private WeaponType Gun;
    [SerializeField]
    private Transform gunParent;
    [SerializeField]
    private List<GunScriptableObject> guns;
    [SerializeField]
    //private PlayerIK playerIK; Player Animation stuff for later

    [Space]
    [Header("Runtime Refernces")]
    public GunScriptableObject activeGun;

    private void Start()
    {
        GunScriptableObject gun = guns.Find(gun => gun.type == Gun);

        if (gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for WeaponType: {gun}");
            return;
        }

        activeGun = gun;
        gun.Spawn(gunParent, this);

        //IK stuff for player animation later
        //Transform[] allChildren = gunParent.GetComponentsInChildren<Transform>();
        //playerIK.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        //playerIK.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        //playerIK.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        //playerIK.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }

}
