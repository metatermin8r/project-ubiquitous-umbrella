using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public UIAnimationController uiAnimationController;

    [Header("Active Inventory")]
    public Item[] items;
    public int weaponIndex;

    [Header("Inventory Item Index")]
    public int itemIndex;
    public int previousItemIndex = -1;
    //public int killFeedHowImage;

    [Header("Bools for Abstract Methods")]
    public bool isSprinting;
    public bool isSliding;
    public bool isZoomed;
    public bool isReloading;

    [Header("Pause Menu")]
    public MenuManager MenuManager;
    public bool pauseMenuActive;

    [Header("Weapon Sway")]
    public FirstPersonWeaponMovement weaponSway;

    // Start is called before the first frame update
    void Start()
    {
        weaponSway = gameObject.GetComponentInChildren<FirstPersonWeaponMovement>();
        MenuManager = gameObject.GetComponentInChildren<MenuManager>();

        //All item prefabs should default to their unequipped states and equipped through code.
        //This call handles that intial equip.
        EquipItem(0);

        if (uiAnimationController == null)
            uiAnimationController = GameObject.Find("Player_Hud/UIAnimatorController").GetComponent<UIAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Handles weapon switching via mouse wheel
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            uiAnimationController.PlaySwapWeaponAnimation();

            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
                EquipItem(itemIndex + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            uiAnimationController.PlaySwapWeaponAnimation();

            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
                EquipItem(itemIndex - 1);
        }

        //Handles weapon switching via keyboard numbers, requires one else if and a keybind per weapon slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }

        //***********************************************************************************************
        //*                                                                                             *
        //*This is what handles Mouse 1 input! This is how you make inputs! REMEMBER THIS SHIT AHHHHHHH!*
        //*                                                                                             *
        //***********************************************************************************************
        if (Input.GetMouseButton(0) && !MenuManager.gameIsPaused && !isSprinting) //&& !pauseMenuActive)
        {
            items[itemIndex].Use();
        }

        if (Input.GetKeyDown(KeyCode.R) && !MenuManager.gameIsPaused && !isSprinting)
        {
            //isReloading = true;
            items[itemIndex].Reload();
        }

        if (Input.GetKey(KeyCode.Mouse1) && !MenuManager.gameIsPaused)
        {
            isZoomed = true;
            items[itemIndex].Zoom();
            weaponSway.adsDisable(); //This really should be just reducing sway values, but disabling them works for now
        }
        else
        {
            isZoomed = false;
            weaponSway.adsEnable(); //This should just reset the sway values back to normal
        }

        if (Input.GetKey(KeyCode.LeftShift) && !MenuManager.gameIsPaused && playerStats.currentStamina > 0 && !isReloading && !isZoomed)
        {
            items[itemIndex].Sprint();
            isSprinting = true;
            playerStats.Sprint();
        }
        else
        {
            //if (playerAnimator != null)
            //playerAnimator.SetBool("Sprinting", false);

            isSprinting = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) && isSprinting)
        {
            items[itemIndex].Slide();
            isSliding = true;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isSliding)
        {
            items[itemIndex].Slide();
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        if (Input.GetKeyDown(KeyCode.F) && !MenuManager.gameIsPaused)
        {
            items[itemIndex].Melee();
        }

        if (Input.GetKeyDown(KeyCode.G) && !MenuManager.gameIsPaused)
        {
            items[itemIndex].Grenade();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !MenuManager.gameIsPaused)
        {
            //items[itemIndex].Super();
        }

        if (Input.GetKeyDown(KeyCode.E) && !MenuManager.gameIsPaused)
        {
            //****************************************
            //*                                      *
            //*Lydia, your item pickup code goes here*
            //*                                      *
            //****************************************
        }

        //This is a very hacky solution to the fact that this code used to call weaponSway.EnableAll every frame
        //That can't happen, so things are handled seperately with roughly the same logic.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Checks if game is paused
            if (!MenuManager.gameIsPaused)
            {
                //Basic call for general inputs
                weaponSway.pmEnableAll();
            }
            else if (MenuManager.gameIsPaused)
            {
                weaponSway.pmDisableAll();
            }
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        weaponIndex = itemIndex;
        items[itemIndex].itemGameObject.SetActive(true);
        //killFeedHowImage = items[itemIndex].GetComponent<SingleShotGun>().killFeedHowImageIndex;
        //items[itemIndex].GetComponent<SingleShotGun>().weaponHandIK.weight = 1.0f;
        items[itemIndex].GetComponent<HitscanWeapon>().fpWeaponHandIK.weight = 1.0f;
        items[itemIndex].GetComponent<HitscanWeapon>().isEquiped = true;

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].GetComponent<HitscanWeapon>().isEquiped = false;
            items[previousItemIndex].itemGameObject.SetActive(false);
            //items[previousItemIndex].GetComponent<SingleShotGun>().weaponHandIK.weight = 0.0f;
            items[previousItemIndex].GetComponent<HitscanWeapon>().fpWeaponHandIK.weight = 0.0f;
        }

        previousItemIndex = itemIndex;

        //if (PV.IsMine)
        //{
        //    Hashtable hash = new Hashtable();
        //    hash.Add("itemIndex", itemIndex);
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //}
    }
}
