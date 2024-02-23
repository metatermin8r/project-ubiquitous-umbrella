using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    public PrimaryWeaponUI primaryWeaponUI;
    public SecondaryWeaponUI secondaryWeaponUI;
    public bool isSwapped;
    // Update is called once per frame
    void Start () 
    { 
        isSwapped = false;
    }
    void Update()
    {
        //if (!isSwapped && Input.GetKeyDown(KeyCode.N))
        //{
        //    primaryWeaponUI.SwapWeaponAnimation();
        //    secondaryWeaponUI.SwapWeaponAnimation();
        //    isSwapped = true;
        //}
        //else if (isSwapped && Input.GetKeyDown(KeyCode.N))
        //{
        //    primaryWeaponUI.SwapBackWeaponAnimation();
        //    secondaryWeaponUI.SwapBackWeaponAnimation();
        //    isSwapped = false;
        //}
    }

    public void PlaySwapWeaponAnimation()
    {
        if (!isSwapped)
        {
            primaryWeaponUI.SwapWeaponAnimation();
            secondaryWeaponUI.SwapWeaponAnimation();
            isSwapped = true;
        }
        else if (isSwapped)
        {
            primaryWeaponUI.SwapBackWeaponAnimation();
            secondaryWeaponUI.SwapBackWeaponAnimation();
            isSwapped = false;
        }
    }
}
