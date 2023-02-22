using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] private GameObject hotbarPanel;
    [SerializeField] private GameObject itemButton;
    [SerializeField] private ToggleGroup toggleGroup;

    private HotbarItem[] items = new HotbarItem[10];
    public GameObject[] starterItems = new GameObject[10];

    private BuildManager buildManager;

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();

        for (int i = 0; i < items.Length; i++)
        {
            if (starterItems[i] != null)
            {
                items[i] = new HotbarItem(starterItems[i]);
            } 
            else
            {
                items[i] = new HotbarItem();
            }
            CreateButton(items[i]);
        }

        
        //items[0].button.GetComponent<Toggle>().
    }

    private void CreateButton(HotbarItem item)
    {
        GameObject button = Instantiate(itemButton, hotbarPanel.transform, false);

        button.GetComponent<Toggle>().group = toggleGroup;
        button.GetComponent<Toggle>().onValueChanged.AddListener((value) => OnValueChanged(value, item));
        button.transform.GetChild(0).GetComponent<Text>().text = item.hasItem ? item.itemName : "Empty";
        if (item.hasItem) button.GetComponent<Image>().sprite = item.itemIcon;

        item.button = button;
    }

    public void OnValueChanged(bool value, HotbarItem item)
    {
        if (value == true && item.hasItem)
        {
            buildManager.SetCurrentObject(item.item);
        }
        else if (value == false)
        {
            buildManager.SetCurrentObject(null);
        }
    }

    public void OnHotbar(int numKey)
    {
        //minus one to get the index
        numKey -= 1;
        //if the 0 key was pressed, make sure we wrap the index around as the 0 button is actually the last slot on the hotbar
        if (numKey == -1) { numKey = 9; }
        items[numKey].button.GetComponent<Toggle>().isOn = true;
    }
}
