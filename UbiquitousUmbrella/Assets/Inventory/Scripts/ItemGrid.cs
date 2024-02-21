using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    // size of individual tiles used in the math to calculate location of tile 
    //will need to change if a different tile size is used!
    public const float tileSizeWidth = 64;
    public const float tileSizeHeight = 64;

    InventoryItem[,] inventoryItemSlot;

    //Allows to read object position (top left point of the grid)
    RectTransform rectTransform;

    //allows you to set the size of the inventory in the editor
    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 7;

    

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];
        //this is so if you select a tile without an intem in it you will not get an error
        if (toReturn == null) { return null; }

        // this is so once an item has been moved to a new position the old position is no longer marked as occupied
        CleanGridReference(toReturn);

        return toReturn;
    }

    private void CleanGridReference(InventoryItem item) 
    {
        //this is so the selectable area of the icon is the same size as the image
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }
    //this sets the size of the inventory at the start
    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    //This is calculating the positions of the mouse hovering on the grid and then the tile position
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {   //grid position
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        //tile position
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);
        return tileGridPosition;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < gridSizeWidth; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (BoundaryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }
        //checks if there is item overlap befor placing the item
        if(OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }
        if(overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }


        PlaceItem(inventoryItem, posX, posY);

        return true;
    }


    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        //this is so the selectable area of the icon is the same size as the image
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        //this calculates the position when placing items
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    //this calculates the position when placing items
    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        // width/ horizontal position
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        // height/ vertical position
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }



    //this checks to see if there is already an item in a slot
    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

        // checks if position is within the grid
        bool PositionCheck(int posX, int posY)
        {
            if (posX < 0 || posY < 0)
            {
                return false;
            }

            if (posX >= gridSizeWidth || posY >= gridSizeHeight)
            {
                return false;
            }

            return true;
        }

        // checks if the entire object is withie grid boundary
        public bool BoundaryCheck(int posX, int posY, int width, int height)
        {
            if (PositionCheck(posX, posY) == false) { return false; }

            posX += width - 1;
            posY += height - 1;

            if (PositionCheck(posX, posY) == false) { return false; }

            return true;
        }

    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {

                    return false;
                }
            }
        }

        return true;
    }
}
 