using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
//this script controls being able to select tiles so that you can interact with them.
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    ItemGrid itemGrid;
    
    public void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
    }

    //detects when the pointer enters the grid and only calculates position of mouse when the pointer
    //is within the grid
    public void OnPointerEnter(PointerEventData eventData) 
    { 
        inventoryController.SelectedItemGrid = itemGrid;
    }

    //detects when the pointer exits the grid
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = null;
    }

}
