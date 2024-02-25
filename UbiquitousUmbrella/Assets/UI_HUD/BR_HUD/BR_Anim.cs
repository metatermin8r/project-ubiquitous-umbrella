using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Anim : MonoBehaviour
{
    [Header("Crosshair References")]
    [SerializeField] HitscanWeapon hitscanWeapon;
    [SerializeField] CH_BR_Anim_top CHTop;
    [SerializeField] CH_BR_Anim_bottom CHBottom;
    [SerializeField] CH_BR_Anim_right CHRight;
    [SerializeField] CH_BR_Anim_left CHLeft;

    // Update is called once per frame
    void Update()
    {
        if (hitscanWeapon.shooting && !hitscanWeapon.reloading)
        {
            CHTop.Shoot();
            CHBottom.Shoot();
            CHLeft.Shoot();
            CHRight.Shoot();
        }
        else if (!hitscanWeapon.shooting) 
        {
            CHTop.Idle();
            CHBottom.Idle();
            CHLeft.Idle();
            CHRight.Idle();

        }
    }
}
