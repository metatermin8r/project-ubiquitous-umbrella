using System;
using UnityEngine;

public class ItemChest : MonoBehaviour
{
    //reference to item to be added to inventory
    //[SerializeField] Item item;
    //reference to inventory
    [SerializeField] InventoryController inventory;
    [SerializeField] KeyCode itemPickupKey = KeyCode.E;

    private bool isInRange;

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(itemPickupKey))
        {
          //  inventory.InsertItem(item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isInRange = true;
    }

    private void OnTriggerExit(Collider other) 
    { 
        isInRange = false;
    }
}
