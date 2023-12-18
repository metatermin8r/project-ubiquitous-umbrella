using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerWeaponSelector weaponSelector;

    private void Update()
    {
        if (weaponSelector.activeGun != null)
        {
            weaponSelector.activeGun.Tick(Input.GetMouseButton(0)); //Has to be this way, looks wrong, weird bool BS
        }
    }
}
