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

    private GameObject dropped;

    public static InventoryController instance;


    public List<ItemData> items = new List<ItemData>();
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    public List<HeldItem> heldItems = new List<HeldItem>();

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();

        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        
    }

    public void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.K))
        {
            InsertRandomItem();
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

        if (Input.GetKeyDown(KeyCode.X))
        {
            //DropItem(item);
            Debug.Log("You have destroyed " + dropped);

            Destroy(selectedItem);
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
    //this doesn't do anything anymore
    public void InsertRandomItem()
    {
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    //use to add items to inventory
    public void InsertItem(InventoryItem itemToInsert)
    {
       Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);
        //this stops more items from being added when the inventory is full
        if (posOnGrid == null)
        {
            return;
        }
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    //this adds and holds the items in the inventory
    public void AddNewItem(HeldItem itemToAdd)
    {

        bool itemExists = false;
        foreach (HeldItem item in heldItems)
        {
            if(item.name  == itemToAdd.name)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>(); //this assigns a data script to the item prefab
                selectedItem = inventoryItem; //stored reference to selected item                    // then create an instance of it in the inventory grid
                                              //...im pretty sure
                rectTransform = inventoryItem.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform);
                rectTransform.SetAsLastSibling();

                // this is getting the object id that correlates to the item on the list in the inventory controller
                // itemPickupable = GetComponent<ItemPickupable>();
                //UnityEngine.Random.Range(itemPickupable.id, itemPickupable.id + 1); //this sets the range from the itemID to the itemID plus one... which results in the itemID always being selected
                int selectedItemID = item.id;
                inventoryItem.Set(items[selectedItemID]);
                Debug.Log("you just picked up item: " + item.id + " with controller");

                item.count += itemToAdd.count;
                itemExists = true;


                break;

            }

        }

        if(!itemExists)
        {
            heldItems.Add(itemToAdd);

            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>(); //this assigns a data script to the item prefab
            selectedItem = inventoryItem; //stored reference to selected item                    // then create an instance of it in the inventory grid
                                          //...im pretty sure
            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
            // this is getting the object id that correlates to the item on the list in the inventory controller
            // itemPickupable = GetComponent<ItemPickupable>();
            //UnityEngine.Random.Range(itemPickupable.id, itemPickupable.id + 1); //this sets the range from the itemID to the itemID plus one... which results in the itemID always being selected
            int selectedItemID = itemToAdd.id;
            inventoryItem.Set(items[selectedItemID]);
            Debug.Log("you just picked up item: " + itemToAdd.id + " with controller");

        }
        Debug.Log(itemToAdd.count + " " + itemToAdd.name + "added to inventory.");
    }

    public void DropItem(HeldItem itemToDrop)
    {

        if (selectedItem != null)
        {
            heldItems.Remove(itemToDrop);
            // this is getting the object id that correlates to the item on the list in the inventory controller
            int selectedItemID = itemToDrop.id;
            //inventoryItem.Set(items[selectedItemID]);
            Debug.Log("you just dropped item: " + itemToDrop.id + " with controller");

        }
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
            //Debug.Log("don't handle highlight, you have nothing selected, Position : " + positionOnGrid.y.ToString() + ", " + positionOnGrid.y.ToString());
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
    //this method generates whatever item was pivked up to the inventory.
    public void CreateRandomItem()
    {
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>(); //this assigns a data script to the item prefab
            selectedItem = inventoryItem; //stored reference to selected item                    // then create an instance of it in the inventory grid
                                          //...im pretty sure
            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
        // this is getting the object id that correlates to the item on the list in the inventory controller
        // itemPickupable = GetComponent<ItemPickupable>();
            //int selectedItemID = item.id; //UnityEngine.Random.Range(itemPickupable.id, itemPickupable.id + 1); //this sets the range from the itemID to the itemID plus one... which results in the itemID always being selected
           // inventoryItem.Set(items[selectedItemID]);
            //Debug.Log("you just picked up item: " + item.id + " with controller");
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
