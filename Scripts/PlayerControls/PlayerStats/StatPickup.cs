using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We use this to make a handy dandy serialized object to make things neater on the storage end 
[CreateAssetMenu(menuName = "Inventory/StatPickup", fileName = "New Item")]
//This derives from our item which is necesarry for the pick up code
public class StatPickup : Item
{
    //This is the value of the picked up statBoost
    public int value;
    //This is the type of the stat 
    public StatGlobals.STAT_ENUM statType;
}
