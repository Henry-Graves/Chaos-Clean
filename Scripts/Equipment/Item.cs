using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    //This is used for things like health, speed, etc. 
        //I.E. stuff thats not one time use.
    public bool isPermaStatBoost = true; 

    public virtual void Use()
    {
        Debug.Log("Using " + name);
    }
}
