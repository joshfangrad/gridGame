using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarItem
{
    public string itemName;
    public Sprite itemIcon;
    public GameObject item;
    public GameObject button;
    public bool hasItem;

    public HotbarItem(GameObject _item = null)
    {
        item = _item;
        hasItem = _item != null;
        itemName = (_item != null) ? _item.name : "Empty";
    }
}
