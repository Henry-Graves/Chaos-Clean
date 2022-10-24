using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cone : EquipmentScript
{

    public float duration = 5f; 

    public override void Activate()
    {
        //The cone is already an obstacle 
        StartCoroutine(DeleteConeAfterDuration(EquipmentValuesManager.instance.coneDuration));
        transform.gameObject.GetComponent<NavMeshObstacle>().size = new Vector3(EquipmentValuesManager.instance.coneRadius, EquipmentValuesManager.instance.coneRadius, EquipmentValuesManager.instance.coneRadius);

    }

    IEnumerator DeleteConeAfterDuration(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    public override bool UpgradeEquipment()
    {
        if (EquipmentValuesManager.instance.coneUpgradeLevel < 3)
        {
            EquipmentValuesManager.instance.coneDuration *= 1.25f;
            EquipmentValuesManager.instance.coneRadius *= 1.3f;
            EquipmentValuesManager.instance.coneUpgradeLevel++;

            if (EquipmentValuesManager.instance.coneUpgradeLevel >= 3)
                return false; 

        return true;
        }
        else
            return false;
    }
}
