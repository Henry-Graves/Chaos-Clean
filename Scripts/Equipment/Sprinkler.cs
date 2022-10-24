using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : EquipmentScript

{
    public GameObject sprinklerDropPrefab;

    private bool hasActivated = false;



    public IEnumerator CheckForGlitch()
    {
        yield return new WaitForSeconds(2f);
        if (!hasActivated)
        {
            //Its already been disabled, so lets remove this object and spawn a pickup instead 
            Instantiate(sprinklerDropPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }


    public override void Activate()
    {
        StartCoroutine(CheckForGlitch());
    }

    private void OnTriggerEnter(Collider collider)
    {
        string tag = collider.tag;

        if (tag == "FilthSpawnableFloor" && isThrown)
        {
            FilthSpawnableFloorController fsfc = collider.GetComponent<FilthSpawnableFloorController>();

            if (fsfc.DisableRoom())
            {
                // Stand upright and lock movement
                transform.rotation = Quaternion.identity;
                transform.Rotate(180, 0, 0);
                transform.position += Vector3.up * 0.125f;
                GetComponent<Rigidbody>().isKinematic = true;
                hasActivated = true;
                gameObject.SetActive(false);
            }
        }
    }
}
