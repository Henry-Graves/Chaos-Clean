using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EquiptmentManager : MonoBehaviour
{
    //Lets just make this a singleton like our stat manager
    //Singleton so we can call else where 
    public static EquiptmentManager instance;

    //Delegate to notify everywhere (mainly for UI) 
    public delegate void OnEquipChange();
    public OnEquipChange onEquipChange;

    //Use this to keep track of the current equiptment
    public int? currentEquiptment = null;

    //This will serve as the ordering of our items, to work as a lookup for our
    //Stores the enum values showing equip type 
    public List<int> equiptmentCycle;

    public GameObject coneDeployedPrefab, soapbombDeployedPrefab, wetfloorDeployedPrefab, roombaDeployedPrefab, sprinklerDeployedPrefab;
    private GameObject cone, soapbomb, wetfloor, roomba, sprinkler;

    //We also need all the prefabs that we've bought 
    public GameObject coneDropPrefab, soapbombDropPrefab, wetfloorDropPrefab, roombaDropPrefab;

    //This shows what the enemies can drop 
    public WeightedRandomList<GameObject> randomDrops = new WeightedRandomList<GameObject>();



    Cone coneScript;
    Soapbomb sbScript;
    Wetfloor wfScript;
    Roomba rmScript;
    Sprinkler spScript;


    public Camera cam;

    public bool shouldThrow;

    [SerializeField]
    //This stores the number of type item we have
    public Dictionary<int, float> equiptments = new Dictionary<int, float>();

    public Dictionary<int, GameObject> prefabLookup = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> deployLookup = new Dictionary<int, GameObject>();

    private void Awake()
    {
        equiptmentCycle = new List<int>();
        shouldThrow = true;

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of equip manager found!!");
            return;
        }

        instance = this;

    }


    private void Start()
    {
        //Lets just get all the prefabs into a dict to make it easier 
        prefabLookup[(int)EquiptmentGlobals.EQUIP_ENUM.CONE] = coneDropPrefab;
        prefabLookup[(int)EquiptmentGlobals.EQUIP_ENUM.SOAPBOMB] = soapbombDropPrefab;
        prefabLookup[(int)EquiptmentGlobals.EQUIP_ENUM.ROOMBA] = roombaDropPrefab;
        prefabLookup[(int)EquiptmentGlobals.EQUIP_ENUM.WETFLOOR] = wetfloorDropPrefab;

        deployLookup[(int)EquiptmentGlobals.EQUIP_ENUM.CONE] = coneDeployedPrefab;
        deployLookup[(int)EquiptmentGlobals.EQUIP_ENUM.ROOMBA] = roombaDeployedPrefab;
        deployLookup[(int)EquiptmentGlobals.EQUIP_ENUM.WETFLOOR] = wetfloorDeployedPrefab;
        deployLookup[(int)EquiptmentGlobals.EQUIP_ENUM.SOAPBOMB] = soapbombDeployedPrefab;

    }


    private void Update()
    {
        if (FreezeController.isFrozen && shouldThrow)
        {

        }
    }

    public void AddEquipt(int equipType, float value = 1)
    {
        //See if the dict already has this equiptment 
        if (equiptments.ContainsKey(equipType))
        {
            equiptments[equipType] += value;
        }
        //otherwise make the key 
        else
        {
            equiptments[equipType] = value;
            //We don't have this equiptment so add it to the end of the list 
            equiptmentCycle.Add(equipType);

        }

        //If we have no items, set this item as our current item
        if (currentEquiptment == null)
        {
            currentEquiptment = 0;
        }

        if (currentEquiptment != null && equiptmentCycle != null && equiptments[equiptmentCycle[(int)currentEquiptment]] <= 0)
            CycleEquiptment();

        //Notify that we've changed our equip 
        if (onEquipChange != null)
            onEquipChange.Invoke();
        else
            Debug.Log("No one listening for equiptment changes!");

        Debug.Log("Current equipment" + currentEquiptment.Value.ToString());
    }


    public void RemoveEquiptment(int equipType, int value = 1)
    {
        //We'll check to make sure they can't go negative elsewhere since we have access to the data
        equiptments[equipType] -= value;

        //A little step around for making the compiler happy when currentEquipt can be null
        if (currentEquiptment != null && equiptmentCycle != null && equiptments[equiptmentCycle[(int)currentEquiptment]] <= 0)
            CycleEquiptment();

        Debug.Log("Current equipment" + currentEquiptment.Value.ToString());


        //Debug.Log("now " + stats[statType]);

        //Notify that we've changed our equip 
        if (onEquipChange != null)
            onEquipChange.Invoke();
        else
            Debug.Log("No one listening for equiptment changes!");
    }

    public void CycleEquiptment()
    {
        //First early stop if they haven't bought enough from shop
        if (equiptmentCycle.Count() < 2)
            return;

        //Second early if they're out of equip or theres not a second value to switch
        bool isValidVal = false;
        //Early terminate 
        foreach (var val in equiptments.Values)
        {
            if (val > 0)
            {
                isValidVal = true;
            }
        }

        //Out of everything, or only 1 thing left
        if (!isValidVal)
            return;



        //Now that we know we have to, we can do the computation heavy stuff
        //If we have more than one item 
        if (currentEquiptment != null)
        {
            //Nice and easy with the list 
            //currentEquiptment += 1;
            //currentEquiptment = currentEquiptment % equiptmentCycle.Count();

            if (currentEquiptment != null && equiptmentCycle != null)
            {
                //If they don't have 
                //Find the index of the key we tried to switch to 
                //Start a loop at that index


                for (int i = 1; i < equiptmentCycle.Count(); i++)
                {
                    //Lets add our current index so we start there 
                    if (equiptments[equiptmentCycle[(int)(i + currentEquiptment) % equiptmentCycle.Count()]] > 0)
                    {
                        currentEquiptment = (i + currentEquiptment) % equiptmentCycle.Count();
                        break;
                    }
                }
            }

            Debug.Log("Current equipment" + currentEquiptment.Value.ToString());


            //Notify that we've changed our equip 
            if (onEquipChange != null)
                onEquipChange.Invoke();
            else
                Debug.Log("No one listening for equiptment changes!");

        }
    }


    public void DeployCurrentEquiptment()
    {

        if (equiptments[equiptmentCycle[(int)currentEquiptment]] <= 0)
            return;


        //This controls how far away from the camera 
        float distance = 1.3f;
        //This should pull up a prefab that we can then pull a script off??
        //Use ID to load a prefab of the object to pull its script off it 

        //This functions gets called twice every time a button is clicked, one on start, one on cancel 
        //So we flip this bool to differentiate ready request vs deploy
        shouldThrow = !shouldThrow;

        // Don't allow deploy if the game is frozen
        if (FreezeController.isFrozen)
        {
            if (cone != null)
                Destroy(cone);

            if (soapbomb != null)
                Destroy(soapbomb);

            if (roomba != null)
                Destroy(roomba);

            if (wetfloor != null)
                Destroy(wetfloor);

            if (sprinkler != null)
                Destroy(sprinkler);
            return;
        }

        //I couldn't think of a clean way to do this off the top of my head and time is crunch mode, so disgusting if string for now
        if (currentEquiptment != null)
        {
            if (equiptmentCycle[(int)currentEquiptment] == (int)EquiptmentGlobals.EQUIP_ENUM.CONE)
            {
                if (!shouldThrow)
                {
                    //Call the cone script
                    //We can dig for this in the lookup array 
                    cone = Instantiate(coneDeployedPrefab, new Vector3(cam.transform.position.x, cam.transform.position.y - 1, cam.transform.position.z) + cam.transform.forward * distance + cam.transform.right, transform.rotation);
                    cone.transform.SetParent(cam.transform, true);
                    coneScript = cone.gameObject.GetComponent<Cone>();
                    coneScript.ShowTragectory();

                }
                else
                {
                    Debug.Log(coneScript);
                    cone.transform.SetParent(null, true);
                    coneScript.Throw();

                }


            }
            else if (equiptmentCycle[(int)currentEquiptment] == (int)EquiptmentGlobals.EQUIP_ENUM.SOAPBOMB)
            {
                if (!shouldThrow)
                {
                    //Call the soapbomb script
                    soapbomb = Instantiate(soapbombDeployedPrefab, new Vector3(cam.transform.position.x, cam.transform.position.y - 1, cam.transform.position.z) + cam.transform.forward * distance + cam.transform.right, cam.transform.rotation);
                    soapbomb.transform.SetParent(cam.transform, true);
                    sbScript = soapbomb.gameObject.GetComponent<Soapbomb>();
                    sbScript.ShowTragectory();
                }
                else
                {

                    Debug.Log(sbScript);
                    sbScript.Throw();
                    soapbomb.transform.SetParent(null, true);
                }



            }
            else if (equiptmentCycle[(int)currentEquiptment] == (int)EquiptmentGlobals.EQUIP_ENUM.WETFLOOR)
            {
                if (!shouldThrow)
                {
                    //Call the wetfloor script
                    wetfloor = Instantiate(wetfloorDeployedPrefab, new Vector3(cam.transform.position.x, cam.transform.position.y - 1, cam.transform.position.z) + cam.transform.forward * distance + cam.transform.right, cam.transform.rotation);
                    wetfloor.transform.SetParent(cam.transform, true);
                    wfScript = wetfloor.gameObject.GetComponent<Wetfloor>();
                    wfScript.ShowTragectory();
                }
                else
                {

                    wfScript.Throw();
                    wetfloor.transform.SetParent(null, true);
                }


            }
            else if (equiptmentCycle[(int)currentEquiptment] == (int)EquiptmentGlobals.EQUIP_ENUM.ROOMBA)
            {
                if (!shouldThrow)
                {
                    //Call the roomba script
                    roomba = Instantiate(roombaDeployedPrefab, new Vector3(cam.transform.position.x, cam.transform.position.y - 1, cam.transform.position.z) + cam.transform.forward * distance + cam.transform.right, cam.transform.rotation);
                    roomba.transform.SetParent(cam.transform, true);
                    rmScript = roomba.gameObject.GetComponent<Roomba>();
                    rmScript.ShowTragectory();

                }
                else
                {

                    rmScript.Throw();
                    roomba.transform.SetParent(null, true);

                }

            }
            else if (equiptmentCycle[(int)currentEquiptment] == (int)EquiptmentGlobals.EQUIP_ENUM.SPRINKLER)
            {
                if (!shouldThrow)
                {
                    //Call the roomba script
                    sprinkler = Instantiate(sprinklerDeployedPrefab, new Vector3(cam.transform.position.x, cam.transform.position.y - 1, cam.transform.position.z) + cam.transform.forward * distance + cam.transform.right, cam.transform.rotation);
                    sprinkler.transform.SetParent(cam.transform, true);
                    spScript = sprinkler.gameObject.GetComponent<Sprinkler>();
                    spScript.ShowTragectory(true);

                }
                else
                {

                    spScript.Throw();
                    sprinkler.transform.SetParent(null, true);

                }

            }

            //All of these only use one item: 
            if (shouldThrow)
                RemoveEquiptment(equiptmentCycle[(int)currentEquiptment], 1);
        }

    }

    //We need to add all of our shop logic 
    //Returns true if worked, false if max upgraded 
    public bool getEquipmentFromShop(int equipRequest, bool isUpgrade)
    {
        //We have special behavior for sprinklers 
        if (equipRequest == (int)EquiptmentGlobals.EQUIP_ENUM.SPRINKLER)
        {
            AddEquipt(equipRequest, 1);
            return true; 
        }
        else
        {

            if (!isUpgrade)
            {
                //We're just going to weight all the items equally 
                randomDrops.Add(prefabLookup[equipRequest], 1);
                AddEquipt(equipRequest, 0);
                return true;
            }
            else
            {
                //We want to pull the script off the object 
                EquipmentScript curScript = deployLookup[equipRequest].GetComponent<EquipmentScript>();




                bool results = curScript.UpgradeEquipment();

                //We need to issue this for updating upgrade numbers 
                if (onEquipChange != null)
                    onEquipChange.Invoke();
                else
                    Debug.Log("No one listening for equiptment changes!");

                return results;
            }
        }
    }

    public void RequestHotKey(int key)
    {
        if (equiptmentCycle != null && equiptments[equiptmentCycle[key]] > 0)
        {
            currentEquiptment = key;
        }

        //Notify that we've changed our equip 
        if (onEquipChange != null)
            onEquipChange.Invoke();
        else
            Debug.Log("No one listening for equiptment changes!");
    }

}
