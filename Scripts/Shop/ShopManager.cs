using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject doorButton;
    private OpenAndCloseSlidingDoors doorController;

    private void Start()
    {
        doorController = doorButton.GetComponent<OpenAndCloseSlidingDoors>();
    }

    // If player enters the defined zone for the shop...
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Freeze the game
            FreezeController.FreezeGame();

            if (!doorController.doorsAreOpen)
                doorController.OpenDoors();
        }
    }

    // If player exits the defined zone for the shop...
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Unfreeze the game and close the door
            doorController.CloseDoors();
            FreezeController.UnFreezeGame();
        }
    }
}
