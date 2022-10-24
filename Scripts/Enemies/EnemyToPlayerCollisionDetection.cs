using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToPlayerCollisionDetection : MonoBehaviour
{
    public EnemyController enemyController;
    public GameObject flashRed;
    public AudioClip playerHurt;

    private void OnTriggerEnter(Collider other)
    {
        // Enemy damaging player
        if (other.tag == "Player" && enemyController.isAttacking)
        {
            // Log the name of the other collider (enemy) being attacked
            PlayerStatController.instance.PlayerTakesDamage((int)enemyController.damage);

            // Hit sound
            AudioSource ac = GetComponent<AudioSource>();
            ac.PlayOneShot(playerHurt);

            // TODO: Make screen flash red - use a mostly transparent 2d canvas layer to avoid shader
            //
        }
    }
}