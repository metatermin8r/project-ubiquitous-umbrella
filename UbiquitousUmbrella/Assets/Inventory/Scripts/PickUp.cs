using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public HeldItem item = new HeldItem("Item Name", 1, 1);

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            InventoryController.instance.AddNewItem(item);
            Destroy(gameObject);
        }
    }
}
