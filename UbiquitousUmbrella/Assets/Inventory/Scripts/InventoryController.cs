using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// when adding function to pick up items in world, it just needs to add an Item Data sciptable object
// to the list on the player camera.

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    //reference to grid tile script
    public ItemGrid selectedItemGrid;

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;


    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    public void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateRandomItem();
        }

        //this is saying to only execute interactions with grid if tile is selected
        if (selectedItemGrid == null) { return; }

        //this selects the picks up items to be moved around
        if (Input.GetMouseButtonDown(1))
        {
            LeftMouseButtonPress();
        }
    }
    //this will need to be replaced with a function for picking up items in the world
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>(); //this assigns a data script to the item prefab
        selectedItem = inventoryItem; //stored reference to selected item                    // then create an instance of it in the inventory grid
                                                                                             //...im pretty sure
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonPress()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.itemData.width - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.itemData.height - 1) * ItemGrid.tileSizeHeight / 2;
        }
        Vector2Int tileGridPosition = selectedItemGrid.GetTileGridPosition(position);
        //if selected item 
        if (selectedItem == null)//only picks up item if one is not already selected
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition) //this is to pick up item within grid, not game
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if(complete)
        {
             selectedItem = null; //this deselects the item
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
        }

    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {   //item position stays at same position as mouse when selected
            rectTransform.position = Input.mousePosition;
        }
    }
}
