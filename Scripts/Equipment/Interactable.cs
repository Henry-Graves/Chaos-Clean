using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    public float radius = 5f;

    Transform player;

    bool isMoving = false;

    Vector3 origPos = new Vector3();
    Vector3 curPos = new Vector3();

    public float spinSpeed = 15f;
    public float maxHeight = .5f;
    public float repitionSpeed = 1f;

    public bool isShowItem = false;

    private bool justSpawned = true;

    public AudioClip dropSound;
    private AudioSource source;



    private void Start()
    {

        source = GetComponent<AudioSource>();

        player = PlayerManager.instance.player.transform;
        if (!isShowItem)
            gameObject.tag = "Interactable";

        SphereCollider collider = transform.GetComponent<SphereCollider>();
        collider.radius = radius;
        collider.isTrigger = true;

        origPos = transform.position;

    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, radius);
    }

    //Note: this is the unique items feature upon reaching the player
    // Therefore, it must be overrriden 
    public virtual void Interact()
    {
        //Debug.Log("Interacting with " + transform);
    }

    public void Update()
    {
        //source != null && dropSound != null
        if (justSpawned && !isShowItem)
        {
            source.clip = dropSound;
            source.PlayOneShot(source.clip, 0.65f);
            justSpawned = false;
        }


        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, player.position.y + 1, player.position.z), 7f * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, new Vector3(player.position.x, player.position.y + 1, player.position.z));
            if (distance < 1.5f)
            {
                Interact();
            }
        }
        else
        {
            //transform.Rotate(new Vector3(0f, Time.deltaTime * spinSpeed, 0f), Space.World);
            curPos = origPos;
            curPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * repitionSpeed) * maxHeight;

            transform.position = curPos;
        }

    }

    //We use this function to get whether or not interaction has been requested 

    public void MoveToInventory()
    {
        isMoving = true;

    }


}
