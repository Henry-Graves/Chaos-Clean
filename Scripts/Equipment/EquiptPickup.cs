using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We use this to make a handy dandy serialized object to make things neater on the storage end 
[CreateAssetMenu(menuName = "Inventory/EquipPickup", fileName = "New Item")]
//This derives from our item which is necesarry for the pick up code
public class EquiptPickup : Item
{
    //If we plan on doing different levels this would be the value
    public int value;

    //We're gonna need a sprite here for the UI

    //This is the type of the stat 
    public EquiptmentGlobals.EQUIP_ENUM equipType;
}
