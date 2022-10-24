using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public DeathMenu deathMenu;
    public PauseMenu pauseMenu;
    //public WinMenu winMenu; 

    public GameObject helpMenu; 

    public GameObject winMenu;

    //We also have an issue with the screens (damage red, and color brown) blocking UI buttons
    //so we'll also just toggle enable and disable from here 

    public GameObject damagePanel;
    public GameObject urgePanel;

    private PlayerControls charCont;
    private CharacterController characterController;
    private MouseLook camController; 

    private bool isGamePaused = false;

    public GameObject floorOneSpawners; 
    public GameObject floorTwoSpawners;


    private int totalSpawners = 0;
    //Score manager will go here 

    //We can include the stat manager here?  

    private void Start()
    {

        //Get the player controls so that we can disable them when the player dies 
        charCont = PlayerManager.instance.player.gameObject.GetComponent<InputManager>().controls;

        characterController = PlayerManager.instance.player.gameObject.GetComponent<CharacterController>();

        camController = PlayerManager.instance.player.gameObject.GetComponent<MouseLook>();

        PlayerStatController.instance.onRoomDisabled += CheckForGameWin; 


        totalSpawners = floorOneSpawners.transform.childCount + floorTwoSpawners.transform.childCount;


    }


    public void CheckForGameWin()
    {
        Debug.Log("Total spawners " + totalSpawners);
        if (PlayerStatController.instance.roomsCleared >= totalSpawners)
        {
            //The player has won the game, so just give them the popup
            RestartGame(false);
        }

    }


    //This is called when the player dies 
    public void RestartGame(bool didDie = true)
    {
        //Safety
        if (deathMenu != null)
        {
            //Re-enable the cursor 
            Cursor.visible = true;

            //Re-enable cursor movement 
            Cursor.lockState = CursorLockMode.None; 

            //Disable the controls 
            charCont.Disable();

            //Disable the char controller so it doesn't get stuck on last input 
            characterController.enabled = false;

            //Stop the camera from spinning uncontrollably if they die while rotating 
            camController.disableCamera();

            urgePanel.SetActive(false);
            damagePanel.SetActive(false);


            //Trigger the deathmenu 
            if (didDie)
                deathMenu.gameObject.SetActive(true);
            else
            {
                Time.timeScale = 0f; 
                winMenu.SetActive(true);
            }

        }
    }

    public void PauseOrUnpauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            PauseGame();
        }
        else
        {
            isGamePaused = false;
            UnPauseGame(); 
        }
    }


    private void PauseGame()
    {
        //Set time = 0 to effectively pause, since 
            //movement is scaled by time, this effectively removes the players controls 
            //This might pose an issue for things not scaled by time in the future tho 
        Time.timeScale = 0f;

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None; 
        //Disable the camera 
        camController.disableCamera();
        pauseMenu.gameObject.SetActive(true);
        urgePanel.SetActive(false);
        damagePanel.SetActive(false);
    }

    private void UnPauseGame()
    {
        //We'll do a save here, we only save 3 values so it shouldn't take long 
        PlayerPrefs.Save(); 

        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1f;
        camController.enableCamera(); 
        pauseMenu.gameObject.SetActive(false);
        //Also set help menu to false incase they were viewing it. 
        helpMenu.SetActive(false);
        urgePanel.SetActive(true);
        damagePanel.SetActive(true);
    }

}
