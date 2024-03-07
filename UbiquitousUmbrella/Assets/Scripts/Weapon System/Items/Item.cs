using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();
    public abstract void Reload();
    public abstract void Zoom();
    public abstract void Sprint();
    public abstract void Slide();
    public abstract void Pickup();
    public abstract void Melee();
    public abstract void Grenade();
}
