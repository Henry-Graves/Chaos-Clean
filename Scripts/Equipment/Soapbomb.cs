using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soapbomb : EquipmentScript
{

    public float delay = .75f;

    public float explosionForce = 99999999f;

    public GameObject explosionEffect;

    public int maxUpgradeLevel = 3;

    public LayerMask layermask;

    public AudioClip explosion;
    private AudioSource soundSource;



    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        soundSource = GetComponent<AudioSource>();
        soundSource.clip = explosion;
        soundSource.PlayOneShot(soundSource.clip);
        Collider[] colliders = Physics.OverlapSphere(transform.position, EquipmentValuesManager.instance.soapbombRadius, layermask);

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.tag == "Enemy")
            {
                EnemyController enemyController = nearbyObject.GetComponent<EnemyController>();
                //Check to ragdoll 
                if (enemyController.ShouldRagDoll(EquipmentValuesManager.instance.soapbombDamage))
                {
                    Rigidbody rb = nearbyObject.gameObject.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        rb.AddExplosionForce(explosionForce, transform.position, 0);
                    }

                }
                else
                    enemyController.TakeDamage(EquipmentValuesManager.instance.soapbombDamage);
            }
            else
            {
                // We've hit filth
                FilthController filth = nearbyObject.GetComponent<FilthController>();
                filth.TierDown();
            }

        }

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject, 3f);
    }

    IEnumerator BombTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Explode();
    }


    public override bool UpgradeEquipment()
    {
        if (EquipmentValuesManager.instance.soapbombUpgradeLevel < maxUpgradeLevel)
        {
            EquipmentValuesManager.instance.soapbombRadius *= 1.07f;
            EquipmentValuesManager.instance.soapbombDamage *= 1.5f;
            EquipmentValuesManager.instance.soapbombUpgradeLevel++;

            //We return false also if the this drove it to max up 
            if (EquipmentValuesManager.instance.soapbombUpgradeLevel >= maxUpgradeLevel)
                return false; 

            return true;
        }
        else
        {
            //They've already max upgraded, so return false
            return false;
        }
    }


    public override void Activate()
    {
        StartCoroutine(BombTimer(delay));

        //startCountdown = true;
        //wait some odd seconds 
        //'Detonate' 
        //Get collision info of everything in a radius 
        //Everything thats an enemy damage 
        //(Bonus: knock them back) 
    }
}
