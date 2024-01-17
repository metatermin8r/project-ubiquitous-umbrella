using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public HeldItem item = new HeldItem("Item Name", 1, 1);
    public GameObject pickUpUI;

    void OnTriggerEnter(Collider other)
    {
        pickUpUI.SetActive(true);

       if (Input.GetKey(KeyCode.E))
        {
            if (other.CompareTag("Player"))
            {
                InventoryController.instance.AddNewItem(item);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        pickUpUI.SetActive(false);
    }
}
