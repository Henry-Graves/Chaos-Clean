using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory instance;

    public delegate void OnInventoryChange();
    public OnInventoryChange onInventoryChangeCallback;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of inventory found!!");
            return;
        }
        instance = this;
    }





    public void Add (Item item)
    {
        if (item == null)
        {
            Debug.Log("item null");
            return;
        }

        if(items.ContainsKey(item))
            items[item] += 1;
        else
        items.Add(item, 1);

        Debug.Log("There are now " + items[item] + " in the dict");

        if (onInventoryChangeCallback != null)
            onInventoryChangeCallback.Invoke();

    }

    public void Remove(Item item) 
    {
        items.Remove(item);

        if (onInventoryChangeCallback != null)
            onInventoryChangeCallback.Invoke();
    }
}
