using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
//Note: we disable this so it doesn't yell at use for using serialized fields 
    //because we can leave these blank and assign them in the editor 
#pragma warning disable 649 

    //Make it public so that we can assign the controlls we wrote 
    // (Keyboard and controller) 
    [SerializeField] CharacterController characterController;

    //Make our variables easy access for things such as speed boost or gravity jumps 
        //We use 0, because 0 is s
    [SerializeField] float speed = 10f;
    [SerializeField] float gravity = -30f;
    [SerializeField] float jumpHeight = 3.5f;

    public GameObject respawnPoint; 

    //We need this ground mask here so that we know when to reset our gravity back to normal 
    [SerializeField] LayerMask groundMask;

    //We need this interactable mask here to check for items 
    [SerializeField] LayerMask interactableMask; 
    
    // Footstep sounds
    public AudioClip[] footstepSounds;
    private AudioSource soundSource;

    //Keep track of the horz input 
    Vector2 horizontalInput;

    //Since we are implementing a free fall, keep track of vert speed
    Vector3 verticalVelocity = Vector3.zero;

    //We keep this as a quick flag to know when to reset gravity  
    bool isGrounded;

    //WE need this bool to prevent the player from jumping when off the ground 
    bool canJump;

    //We use this to denote when the player is trying to interact
    bool interactRequest;

    public GameObject UIShopPanel;
    private ShopPopupManager pop;


    //Delegate to notify everywhere (mainly for UI) 
    public delegate void OnStallEntered(int equipType);
    public OnStallEntered onStallEntered;


    //Delegate to notify everywhere (mainly for UI) 
        //UI then has a reference to movement
    public delegate void OnStallExit(int equipType);
    public OnStallExit onStallExit;


    void Start()
    {

        PlayerStatController.instance.onStatChange += OnNewStatPickup;
        //Get the starting speed
        speed = PlayerStatController.instance.getScaledSpeed();

        soundSource = GetComponent<AudioSource>();

        pop = UIShopPanel.gameObject.GetComponent<ShopPopupManager>();
    }

    //Update all the goods 
    void OnNewStatPickup()
    {
        //A stat changed so lets update via the hash values
        speed = PlayerStatController.instance.getScaledSpeed();
    }


    //Note: Fixed update isn't necesary here as im following a more classic physics standpoint and keeeping track of the time 
    // this counteracts the issue of different frame rates on different devices and makes gravity more accurate. 
    private void Update()
    {
        //We need to check whether or not to reset gravity 
         //So we cast a sphere that checks collision with the ground layer 
        isGrounded = Physics.CheckSphere(transform.position, 0.3f, groundMask); 


        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = 0; 
        }


        //right controlls left and right, forward controlls forward and back, so we get the overall vector of input 
        // then we multiply it by the speed that we want 
        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * PlayerStatController.instance.getScaledSpeed();

        //let unity know the moves we made 
        characterController.Move(horizontalVelocity * Time.deltaTime);

        // Play player walking sound
        if (!(horizontalVelocity.Equals(Vector3.zero)) && soundSource.isPlaying == false && isGrounded)
        {
            int n = Random.Range(1, footstepSounds.Length-1);
            soundSource.clip = footstepSounds[n];
            soundSource.PlayOneShot(soundSource.clip);

            footstepSounds[n] = footstepSounds[0];
            footstepSounds[0] = soundSource.clip;
        }

        //See if player pressed the jump button
        if (canJump)
        {
            //And make sure they're on the ground
            if (isGrounded)
            {
                //Then we perform the jump 
                verticalVelocity.y = Mathf.Sqrt(-2f  * jumpHeight * gravity);
                soundSource.clip = footstepSounds[footstepSounds.Length-1];
                soundSource.PlayOneShot(soundSource.clip);
            }
            //Reset the button press
           canJump = false;
        }

        //here we are managing the players gravity. Note: that we multiply by time.delta twice as it's 
        // the physics equation for free fall. 
        verticalVelocity.y += gravity * Time.deltaTime; 
        //We pass the speeds that we want to our char controller 
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    public void RecieveInput (Vector2 _horizontalInput)
    {
        //Lets get the horizontal input 
        horizontalInput = _horizontalInput;
    }

    public void OnJumpPressed()
    {
        canJump = true; 
    }

    public void RequestInteract()
    {
        interactRequest = true;
        //Debug.Log("Start");
    }

    public void CancelInteract()
    {
        interactRequest = false;
        //Debug.Log("Stop");
    }

    //Check for all triggered events
    private void OnTriggerStay(Collider other)
    {
        // Specifically check if its interactable, and the player is trying to interact
        if (other.tag == "Interactable" && !FreezeController.isFrozen)
        {
            //If they are get the game reference
            var myScript = other.gameObject.GetComponent<Interactable>();
            //Move the item to our inventory
            myScript.MoveToInventory();
        }

        //We put this here so it doesn't matter how long they've been in the shop 
        if (other.tag == "PurchaseBooth" && interactRequest)
        {
            other.GetComponent<ShopPickup>().TryBuyItem(); 
            interactRequest = false;
        }

    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PurchaseBooth")
        {
            
            pop.enablePanel((int)other.gameObject.GetComponent<ShopPickup>().equipType, other.gameObject.GetComponent<ShopPickup>().isUpgradable);
        }

        if (other.tag == "OOB")
        {
            //Movement is attached to player 
            Debug.Log("Ttriggered");
            Debug.Log("CurPos: " + gameObject.transform.position);
            Debug.Log("RequestedPos: " + respawnPoint.transform.position);

            CharacterController cc = PlayerManager.instance.player.gameObject.GetComponent<CharacterController>();

            cc.enabled = false;
            PlayerManager.instance.player.transform.position = new Vector3(respawnPoint.transform.position.x, respawnPoint.transform.position.y, respawnPoint.transform.position.z);
            cc.enabled = true;
            Debug.Log("NewPos: " + gameObject.transform.position);


            //Set transform to respawn point 
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PurchaseBooth")
        {
            pop.disablePanel((int)other.gameObject.GetComponent<ShopPickup>().equipType, other.gameObject.GetComponent<ShopPickup>().isUpgradable);
        }

    }

}
