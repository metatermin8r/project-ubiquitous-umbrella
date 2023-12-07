using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerWeaponSelector weaponSelector;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && weaponSelector.activeGun != null)
        {
            weaponSelector.activeGun.Shoot();
        }
    }
}
