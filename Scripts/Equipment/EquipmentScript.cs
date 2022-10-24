using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentScript : MonoBehaviour
{
    public float strength = .5f;
    public int pointsOnLine = 50;
    public float timeBetweenPoints = 0.1f;
    public float gravity = -20f;

    bool shouldShowTragectory = false;
    bool shouldResetRotation = false;

    private bool shouldShowSpawnable = false;

    LineRenderer lineRenderer;

    public LayerMask CollidableLayers;

    public bool isThrown = false;
    private bool notCollided = true;
    Camera cam; 

    protected virtual void PostStart()
    {
        // Override for children
        // Called immediately after it's parent's start
    }
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        PostStart();
    }

    protected virtual void PostUpdate()
    {
        // Override for children
        // Called after every Update
    }

    private void Update()
    {
        if (shouldShowTragectory)
        {
            cam = transform.parent.gameObject.GetComponent<Camera>();
            lineRenderer.positionCount = pointsOnLine; 
            List<Vector3> points = new List<Vector3> ();
            Vector3 startingPosition = transform.position;
            Vector3 startingVelocity = cam.transform.forward * strength;

            for (float t = 0; t < pointsOnLine; t+= timeBetweenPoints)
            {
                Vector3 curPoint = startingPosition + t * startingVelocity;
                curPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t; 
                points.Add(curPoint);

                //Check if we've hit something the item shouldn't go through 
                var results = Physics.OverlapSphere(curPoint, .1f, CollidableLayers);
                if (results.Length > 0)
                {
                    //For the sprinkler, our only collidable layers will be default and the ground layer 
                    //Lets filter out only the spawnable tag
                    if (shouldShowSpawnable)
                    {
                        
                        foreach (var collider in results)
                        {
                            Debug.Log(collider.tag);
                            if (collider.tag == "FilthSpawnableFloor")
                            {
                                
                                collider.GetComponent<FilthSpawnableFloorController>().showRender();
                            }
                        }
                    }


                    lineRenderer.positionCount = points.Count;
                    break; 
                }
            }

            lineRenderer.SetPositions(points.ToArray());

        }

        PostUpdate();
    }


    // Start is called before the first frame update
    public virtual void Activate()
    {
        //Debug.Log("Bang"); 
    }

    // If the child wants to redefine collision behavior, let them
    protected virtual void OnCollide(Collision collision)
    {
        if (isThrown && notCollided)
        {
            transform.GetComponent<Rigidbody>().velocity *= 0.2f;
            Activate();
            notCollided = false;
        }
    }


    //We'll use this to control each individual equipments upgrade. 
    public virtual bool UpgradeEquipment()
    {
        return false; 
    }

    // Simply call our virtual function
    public void OnCollisionEnter(Collision collision)
    {
        OnCollide(collision);
    }

    public void Throw()
    {
        Rigidbody rb = transform.GetComponent<Rigidbody>();

        if (shouldResetRotation)
        {
            transform.rotation = Quaternion.identity;
            rb.freezeRotation = true;
        }

        rb.isKinematic = false;

        isThrown = true;
        // Turn on the gravity 
        lineRenderer.positionCount = 0;
        shouldShowTragectory = false;
        shouldShowSpawnable = false;
      
        rb.useGravity = true;
        rb.velocity = cam.transform.forward * strength;
    }

    public void ShowTragectory(bool isSprinkler = false)
    {
        shouldShowSpawnable = isSprinkler;
        shouldShowTragectory=true;
    }

    public void ResetRotation()
    {
        shouldResetRotation = true;
    }
}
