using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupable : MonoBehaviour
{
    public GameObject itemObject;
    public InventoryController inventoryController;
    public GameObject pickUpUI;
    // public GameObject Inventory;
    //public int selectedItemID;

    //reference to scriptable object
    public ItemData itemData;

    void Start()
    {
        pickUpUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pickUpUI.SetActive(true);
            if (Input.GetKey(KeyCode.E)) { 
               // Inventory.SetActive(true);
                inventoryController.CreateRandomItem();
                Debug.Log("you just picked up item:" + itemData.itemID);
                if (itemObject != null)
                {
                    this.itemObject.SetActive(false);
                    pickUpUI.SetActive(false);
                }
                Debug.Log("Destroy object!");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        pickUpUI.SetActive(false);
    }
}
