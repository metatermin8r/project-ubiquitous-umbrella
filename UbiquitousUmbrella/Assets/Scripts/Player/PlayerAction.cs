using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;

    [Header("Active Inventory")]
    [SerializeField] Item[] items;
    public int weaponIndex;

    [Header("Inventory Item Index")]
    public int itemIndex;
    int previousItemIndex = -1;
    //public int killFeedHowImage;

    [Header("Bools for Abstract Methods")]
    public bool isSprinting;
    public bool isZoomed;
    public bool isReloading;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
                EquipItem(itemIndex + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
                EquipItem(itemIndex - 1);
        }

        //***********************************************************************************************
        //*                                                                                             *
        //*This is what handles Mouse 1 input! This is how you make inputs! REMEMBER THIS SHIT AHHHHHHH!*
        //*                                                                                             *
        //***********************************************************************************************
        if (Input.GetMouseButton(0)) //&& !pauseMenuActive)
        {
            items[itemIndex].Use();
        }

        if (Input.GetKeyDown(KeyCode.R) && !playerMovement.pauseMenuActive && !isSprinting)
        {
            //isReloading = true;
            items[itemIndex].Reload();
        }

        if (Input.GetKey(KeyCode.Mouse1) && !playerMovement.pauseMenuActive)
        {
            isZoomed = true;
            items[itemIndex].Zoom();
        }
        else
        {
            isZoomed = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && !playerMovement.pauseMenuActive && playerStats.currentStamina > 0 && !isReloading && !isZoomed)
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

        if (Input.GetKeyDown(KeyCode.F) && !playerMovement.pauseMenuActive)
        {
            items[itemIndex].Melee();
        }

        if (Input.GetKeyDown(KeyCode.G) && !playerMovement.pauseMenuActive)
        {
            items[itemIndex].Grenade();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !playerMovement.pauseMenuActive)
        {
            //items[itemIndex].Super();
        }

        if (Input.GetKeyDown(KeyCode.E) && !playerMovement.pauseMenuActive)
        {
            //Pickup code goes here
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (pauseMenuActive)
        //    {
        //        pm.Resume();
        //        pauseMenuActive = false;
        //    }

        //    else
        //    {
        //        pm.Pause();
        //        pauseMenuActive = true;
        //    }
        //}
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
        //items[itemIndex].GetComponent<SingleShotGun>().fpWeaponHandIK.weight = 1.0f;

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
            //items[previousItemIndex].GetComponent<SingleShotGun>().weaponHandIK.weight = 0.0f;
            //items[previousItemIndex].GetComponent<SingleShotGun>().fpWeaponHandIK.weight = 0.0f;
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
