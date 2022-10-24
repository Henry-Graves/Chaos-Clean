using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomba : EquipmentScript
{
    // How long it should take to run the flatten animation;
    public float flattenTime = 0.25f;

    // Mask to the ground
    public LayerMask groundMask;

    // Time to live for
    public float lifetime = 7;

    // Keep the first time we hit the ground, so we can despawn.
    private float startTime = -1;

    private Rigidbody body;

    // Lock our rotation for roomba
    protected override void PostStart()
    {
        body = transform.gameObject.GetComponent<Rigidbody>();
        ResetRotation();
    }

    protected override void PostUpdate()
    {
        // Update is grounded and log startTime if necessary
        if (!isThrown)
            return;

        if (startTime == -1)
            startTime = Time.time;
        else
        {
            // Kill us if we've existed too long
            float curTime = Time.time;
            if (curTime - startTime > EquipmentValuesManager.instance.roombaDuration)
                Destroy(gameObject);
        }

        if (body.velocity.magnitude < 3f)
        {
            transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 45, transform.rotation.z));
            body.AddForce(transform.forward * 7f, ForceMode.VelocityChange);
        }
    }

    IEnumerator FlattenEnemy(GameObject enemy)
    {
        // Scale from full height down.
        Vector3 start = enemy.transform.localScale;
        Vector3 dest = start - new Vector3(0, start.y, 0);

        float startTime = Time.time;
        while ((Time.time - startTime) < flattenTime)
        {
            float timeSpent = Time.time - startTime;
            if (enemy != null)
                enemy.transform.localScale = Vector3.Lerp(start, dest, timeSpent / flattenTime);
            yield return null;
        }

        if (enemy != null)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            ec.Die();
        }
    }

    protected override void OnCollide(Collision collision)
    {
        string tag = collision.collider.tag;
        if (tag == "Filth" || tag == "Enemy")
        {
            // Don't collide
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            if (tag == "Filth")
            {
                // Scale us down, and force a destroy
                FilthController fc = collision.collider.gameObject.GetComponent<FilthController>();
                if (fc.GetScaling()) return;
                fc.TierDown(true);
            }
            else if (tag == "Enemy")
            {
                // Flatten it
                StartCoroutine(FlattenEnemy(collision.collider.gameObject));
            }
        }
    }

    public override bool UpgradeEquipment()
    {
        if (EquipmentValuesManager.instance.roombaUpgradeLevel < 3)
        {
            EquipmentValuesManager.instance.roombaDuration *= 1.5f;
            EquipmentValuesManager.instance.roombaUpgradeLevel++;

            if( EquipmentValuesManager.instance.roombaUpgradeLevel >= 3)
                return false; 

            return true;
        }
        else
            return false;
    }
}
