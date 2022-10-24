using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStatDrop : MonoBehaviour
{
    public static GameObject[] possibleStatDrops;

    //We will add all the prefabs from the inspector 
    public WeightedRandomList<GameObject> statTable;
    private Transform ePos;

    public int dropPercent0To99 = 34; 

    private void Start()
    {
        ePos = GetComponent<Transform>(); 
    }


    public bool maybeDropStat()
    {
        // Roll the die, using 50 chance
        int rand = RandomProvider.random.Next(0, 99);

        //Lets update the drop percent 



        int power = Mathf.FloorToInt(PlayerStatController.instance.maxZombies / 100);
        int tempDrop;

        if (PlayerStatController.instance.maxZombies > 800)
        {
            tempDrop = Mathf.FloorToInt(dropPercent0To99 * Mathf.Pow(.8f, 8));
        }
        else
            tempDrop = Mathf.FloorToInt(dropPercent0To99 * Mathf.Pow(.8f, power));


        if (rand <= tempDrop)
        {
            //Now see if we should drop a stat or item 
            if (rand <= (int)(49 / 2) && EquiptmentManager.instance.randomDrops.Length() > 0)
            {
                //Drop item 
                Instantiate(EquiptmentManager.instance.randomDrops.GetRandom(), new Vector3(ePos.position.x, ePos.position.y + 1, ePos.position.z), ePos.rotation);
            }
            else
            {
                // We want to drop a prefab at this location 
                Instantiate(statTable.GetRandom(), new Vector3(ePos.position.x, ePos.position.y + 1, ePos.position.z), ePos.rotation);
            }
            return true;
        }
        else return false;
    }

    
  

}
