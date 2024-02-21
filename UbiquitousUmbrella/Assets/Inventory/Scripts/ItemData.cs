using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ItemData : ScriptableObject
{
    public string reference;
    public int width = 1;
    public int height = 1;
    public int itemID = 1;

    public Sprite itemIcon;


}
