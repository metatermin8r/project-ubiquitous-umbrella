using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// when adding function to pick up items in world, it just needs to add an Item Data sciptable object
// to the list on the player camera.

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    //reference to grid tile script
    private ItemGrid selectedItemGrid;

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(SelectedItemGrid);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;


    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    public void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.K))
        {
            InsertRandomItem();
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            if (selectedItem == null)
            {
                CreateRandomItem();
            }
           
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            RotateItem();
        }

        //this is saying to only execute interactions with grid if tile is selected
        if (selectedItemGrid == null) 
        {
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();
        //this selects the picks up items to be moved around
        if (Input.GetMouseButtonDown(1))
        {
            LeftMouseButtonPress();
        }
    }

    private void RotateItem() 
    {
        if(selectedItem == null)
        {
            return;
        }

        selectedItem.Rotate();
    }

    //this is probably where you will need to set up picking up items from world
    private void InsertRandomItem()
    {
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
       Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);
        //this stops more items from being added when the inventory is full
        if (posOnGrid == null)
        {
            return;
        }
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;

    private void HandleHighlight()
    {   //getting x and y positions of mouse on from 
        Vector2Int positionOnGrid = GetTileGridPosition();

        if (oldPosition == positionOnGrid) { return; }

        oldPosition= positionOnGrid;
        if (selectedItem == null)
        {
            Debug.Log("don't handle highlight, you have nothing selected, Position : " + positionOnGrid.y.ToString() + ", " + positionOnGrid.y.ToString());
            //this is broken, I have no idea why.
            // if you start the game with the cursor outside of the game window it will throw an error!!
            //fix this so it doesnt try to run if cursor is out of range
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        //this is so highlight follows item while being dragged
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundaryCheck(
                positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH, 
                selectedItem.HEIGHT)
                );

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }


    //this will need to be replaced with a function for picking up items in the world
    public void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>(); //this assigns a data script to the item prefab
        selectedItem = inventoryItem; //stored reference to selected item                    // then create an instance of it in the inventory grid
                                                                                             //...im pretty sure
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
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
                rectTransform.SetAsLastSibling();
            }
        }

    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }


    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {   //item position stays at same position as mouse when selected
            rectTransform.position = Input.mousePosition;
        }
    }
}
