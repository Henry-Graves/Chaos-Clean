using UnityEngine;

public class ItemPickup : Interactable
{

    public StatPickup statPickup;
    public EquiptPickup equipPickup;



    public override void Interact()
    {
        base.Interact();

        PickUp();
    }


    void PickUp()
    {
        if (statPickup == null && equipPickup != null)
        {
            EquiptmentManager.instance.AddEquipt((int)equipPickup.equipType);
        }
        else
        {
            if (statPickup != null)
            {
                PlayerStatController.instance.UpdateStat((int)statPickup.statType, statPickup.value);      
            }
        }

        // Otherwise, add to an inventory instead. 
        Destroy(gameObject);
    }
}
