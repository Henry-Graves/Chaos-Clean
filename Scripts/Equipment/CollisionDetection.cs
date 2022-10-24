using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject hitParticle;
    public AudioClip HitSound;

    IEnumerator DestroyBloodCo(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && weaponController.isAttacking)
        {
            // Log the name of the other collider (enemy) being attacked
            Debug.Log(other.name);
            other.GetComponent<Animator>().SetTrigger("GetHit");
            var curEnemyScript = other.GetComponent<EnemyController>(); 

            if (curEnemyScript != null)
            {
                curEnemyScript.TakeDamage(PlayerStatController.instance.GetDamage()); 
            }
            
            // Hit sound
            AudioSource ac = GetComponent<AudioSource>();
            ac.PlayOneShot(HitSound);

            // Instantiate a hit particle (blood)
            GameObject blood = Instantiate(hitParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z), other.transform.rotation);
            StartCoroutine(DestroyBloodCo(blood));
        }
    }
}
