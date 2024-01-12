using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataHolder : MonoBehaviour
{

    public ItemData itemData;
    public int ItemNum;
    // Start is called before the first frame update
    void Awake()
    {
        int ItemNum = itemData.itemID;
    }

    void Start()
    {
        int ItemNum = itemData.itemID;
        Debug.Log(ItemNum);
    }

}
