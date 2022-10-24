using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentValuesManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static EquipmentValuesManager instance;

    public float soapbombRadius = 5f;
    public float soapbombDamage = 150f;
    public int soapbombUpgradeLevel = 0;

    public float roombaDuration = 7f;
    public int roombaUpgradeLevel = 0;

    public float wetfloorRadius = 5f;
    public float wetfloorDuration = 5f;
    public int wetfloorUpgradeLevel = 0;

    public float coneRadius = 2f;
    public float coneDuration = 7f;
    public int coneUpgradeLevel = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of player stats found!!");
            return;
        }

        instance = this;
    }


    void Start()
    {
        //Initialize the default values hard so it resets 
        soapbombRadius = 5f;
        soapbombDamage = 150f;
        soapbombUpgradeLevel = 0; 

        roombaDuration = 7f;
        roombaUpgradeLevel = 0; 

        wetfloorRadius = 5f;
        wetfloorDuration = 5f;
        wetfloorUpgradeLevel = 0; 

        coneRadius = 2f;
        coneDuration = 7f;
        coneUpgradeLevel = 0;

    }


    public int GetItemRank(int equipId)
    {
        if (equipId == (int)EquiptmentGlobals.EQUIP_ENUM.ROOMBA)
        {
            return roombaUpgradeLevel;
        }
        else if (equipId == (int)EquiptmentGlobals.EQUIP_ENUM.SOAPBOMB)
        {
            return soapbombUpgradeLevel;
        }
        else if (equipId == (int)EquiptmentGlobals.EQUIP_ENUM.WETFLOOR)
        {
            return wetfloorUpgradeLevel;
        }
        else if (equipId == (int)EquiptmentGlobals.EQUIP_ENUM.CONE)
        {
            return coneUpgradeLevel;
        }
        else
            return 0; 
    }


}
