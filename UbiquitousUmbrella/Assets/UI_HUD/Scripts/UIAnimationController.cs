using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    public PrimaryWeaponUI primaryWeaponUI;
    public SecondaryWeaponUI secondaryWeaponUI;
    public bool isSwapped;
    public GameObject[] weaponUiIndex;
    private int prevIndex;

    //script references
    public PlayerAction playerAction;

    //hud references
    public GameObject assaultRifleHUD;
    public GameObject battleRifleHUD;
    public GameObject shotgunHUD;
    public GameObject pistolHUD;
    public GameObject sniperRifleHUD;
    public GameObject runningHUD;


    void Awake()
    {
        if (playerAction == null)
            playerAction = GameObject.Find("Player").GetComponent<PlayerAction>();

        if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.AssaultRifle)
        {
            assaultRifleHUD.SetActive(true);
            battleRifleHUD.SetActive(false);
            shotgunHUD.SetActive(false);
            sniperRifleHUD.SetActive(false);
            pistolHUD.SetActive(false);
            runningHUD.SetActive(false);
        }

        else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.BattleRifle)
        {
            battleRifleHUD.SetActive(true);
            assaultRifleHUD.SetActive(false);
            shotgunHUD.SetActive(false);
            sniperRifleHUD.SetActive(false);
            pistolHUD.SetActive(false);
            runningHUD.SetActive(false);
        }

        else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.Shotgun)
        {
            battleRifleHUD.SetActive(false);
            assaultRifleHUD.SetActive(false);
            shotgunHUD.SetActive(true);
            sniperRifleHUD.SetActive(false);
            pistolHUD.SetActive(false);
            runningHUD.SetActive(false);
        }

        else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.Pistol)
        {
            battleRifleHUD.SetActive(false);
            assaultRifleHUD.SetActive(false);
            shotgunHUD.SetActive(false);
            sniperRifleHUD.SetActive(false);
            pistolHUD.SetActive(true);
            runningHUD.SetActive(false);
        }

        else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.SniperRifle)
        {
            battleRifleHUD.SetActive(false);
            assaultRifleHUD.SetActive(false);
            shotgunHUD.SetActive(false);
            sniperRifleHUD.SetActive(true);
            pistolHUD.SetActive(false);
            runningHUD.SetActive(false);
        }
    }
    // Update is called once per frame
    void Start () 
    { 
        isSwapped = false;
        //weaponUiIndex[playerAction.itemIndex].SetActive(true);
        //weaponUiIndex[playerAction.previousItemIndex].SetActive(false);
    }
    void Update()
    {
        ChangeWeaponUI();
    }

    public void PlaySwapWeaponAnimation()
    {
        if (!isSwapped)
        {
            primaryWeaponUI.SwapWeaponAnimation();
            secondaryWeaponUI.SwapWeaponAnimation();
            isSwapped = true;

            //ChangeWeaponUI();
        }
        else if (isSwapped)
        {
            primaryWeaponUI.SwapBackWeaponAnimation();
            secondaryWeaponUI.SwapBackWeaponAnimation();
            isSwapped = false;

            //ChangeWeaponUI();
        }
    }


    public void ChangeWeaponUI()
    {
        if (playerAction.itemIndex >= 0 && !playerAction.isSprinting)
        {
            if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.AssaultRifle)
            {
                assaultRifleHUD.SetActive(true);
                battleRifleHUD.SetActive(false);
                shotgunHUD.SetActive(false);
                sniperRifleHUD.SetActive(false);
                pistolHUD.SetActive(false);
                runningHUD.SetActive(false);
            }

            else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.BattleRifle)
            {
                battleRifleHUD.SetActive(true);
                assaultRifleHUD.SetActive(false);
                shotgunHUD.SetActive(false);
                sniperRifleHUD.SetActive(false);
                pistolHUD.SetActive(false);
                runningHUD.SetActive(false);
            }

            else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.Shotgun)
            {
                battleRifleHUD.SetActive(false);
                assaultRifleHUD.SetActive(false);
                shotgunHUD.SetActive(true);
                sniperRifleHUD.SetActive(false);
                pistolHUD.SetActive(false);
                runningHUD.SetActive(false);
            }

            else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.Pistol)
            {
                battleRifleHUD.SetActive(false);
                assaultRifleHUD.SetActive(false);
                shotgunHUD.SetActive(false);
                sniperRifleHUD.SetActive(false);
                pistolHUD.SetActive(true);
                runningHUD.SetActive(false);
            }

            else if (playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType == Gun.WeaponType.SniperRifle)
            {
                battleRifleHUD.SetActive(false);
                assaultRifleHUD.SetActive(false);
                shotgunHUD.SetActive(false);
                sniperRifleHUD.SetActive(true);
                pistolHUD.SetActive(false);
                runningHUD.SetActive(false);
            }


            //Debug.Log("you are currently holding " + playerAction.items[playerAction.itemIndex].GetComponent<HitscanWeapon>().weaponType + " in slot " + playerAction.itemIndex);
        } 
        else if (playerAction.isSprinting)
        {
            battleRifleHUD.SetActive(false);
            assaultRifleHUD.SetActive(false);
            shotgunHUD.SetActive(false);
            sniperRifleHUD.SetActive(false);
            pistolHUD.SetActive(false);
            runningHUD.SetActive(true);
        }
    }
}