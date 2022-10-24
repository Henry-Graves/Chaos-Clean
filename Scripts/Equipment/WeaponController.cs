using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Weapon;
    public bool canAttack = true;
    public bool canClean = true;
    public float attackCooldown = 0.5f;
    public float cleanCooldown = 0.6f;
    public AudioClip WeaponAttackSound;
    public AudioClip CleanSound;
    public bool isAttacking = false;
    public bool isCleaning = false;

    public void WeaponAttack()
    {
        isAttacking = true;
        canAttack = false;
        Animator anim = Weapon.GetComponent<Animator>();
        anim.SetTrigger("PlayerAttack");
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(WeaponAttackSound);
        StartCoroutine(ResetCooldown(attackCooldown, true));
    }

    public void WeaponClean()
    {
        isCleaning = true;
        PlayerStatController.instance.isCleaning = isCleaning;
        canClean = false;
        Animator anim = Weapon.GetComponent<Animator>();
        anim.SetTrigger("PlayerClean");
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(CleanSound);
        StartCoroutine(ResetCooldown(cleanCooldown, false));
    }

    IEnumerator ResetCooldown(float cooldownTime, bool attacking)
    {
        yield return new WaitForSeconds(cooldownTime);
        if (attacking)
        {
            isAttacking = false;
            canAttack = true;
        }
        else
        {
            isCleaning = false;
            PlayerStatController.instance.isCleaning = isCleaning;
            canClean = true;
        }
    }

    public void RequestClean()
    {
        if (canClean && !FreezeController.isFrozen)
        {
            WeaponClean();
        }
    }
    public void RequestAttack()
    {
        if (canAttack && !FreezeController.isFrozen)
        {
            WeaponAttack();
        }
    }
}
