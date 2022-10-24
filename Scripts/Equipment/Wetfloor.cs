using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wetfloor : EquipmentScript
{
    public float duration = 5f;

    public LayerMask enemyMask;

    //This should be the same as the trigger raidus 
    public float radius = 5f; 

    public override void Activate()
    {
        //The cone is already an obstacle 
        //Slight change from last iteration by unlocking perma wet floor by holding it out 
        // So we're gonna change by turning on our Trigger at run time 
        StartCoroutine(DeleteWetFloorAfterDuration(EquipmentValuesManager.instance.wetfloorDuration));

        //We need to manual reset ones still in the radius so they don't wander 
        //Overlap sphere the size of radius 
        //Access manual override in script 

    }

    IEnumerator DeleteWetFloorAfterDuration(float waitTime)
    {
        //Set everything up 
        SphereCollider sc = transform.gameObject.GetComponent<SphereCollider>();
        //These two should already be set, but lets just be safe 
        sc.radius = EquipmentValuesManager.instance.wetfloorRadius;
        sc.isTrigger = true; 
        sc.enabled = true;

        Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();

        transform.rotation = new Quaternion(0, 0, 0, 0);

        rb.isKinematic = true;

        //Make it effect all enemies already in the radius 
        OverrideSplipState(true, 1);

        yield return new WaitForSeconds(waitTime);

        //If it gets destroyed while they're in the radius then reset em
            //Since this is gaurenteed to only set false, add a padding for extra security
        OverrideSplipState(false, 1);

        Destroy(gameObject);
    }

    private void OverrideSplipState(bool slipState, int radiusBoost = 0)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius + radiusBoost, enemyMask);
        foreach (Collider c in enemies)
        {
            c.gameObject.GetComponent<EnemyController>().ManualSlipSet(slipState);
        }
    }

    public override bool UpgradeEquipment()
    {
        if (EquipmentValuesManager.instance.wetfloorUpgradeLevel < 3)
        {
            EquipmentValuesManager.instance.wetfloorDuration *= 1.15f;
            EquipmentValuesManager.instance.wetfloorRadius *= 1.2f;
            EquipmentValuesManager.instance.wetfloorUpgradeLevel++;

            if (EquipmentValuesManager.instance.wetfloorUpgradeLevel >= 3)
                return false;

            return true; 
        }
        else
            return false;
    }
}
