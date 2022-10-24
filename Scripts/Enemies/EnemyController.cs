using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Radius at which the enemy detects the player
    public float detectionRadius = 20f;

    Transform target;
    NavMeshAgent agent;
    Animator anim;


    //Hard coded the health, we will probably want to change this 
    [SerializeField]
    private float enemyHealth = 100;
    public float attackCooldown = 2f;
    public float waitTimeBeforeAttack = 3f;
    public int pointWorth = 100;
    public float speed = 5f;
    public float damage = 5f; 

    //Common functionality
    public bool canAttack = true;
    public bool isAttacking = false;
    private bool isFirstAttack = true;
    private bool slipNSlide = false;
    public float ragdollTime = 1.25f; 
    //We're just gonna pick a big number 
    public float distanceForward = 1.5f;
    // Zombie sounds
    public AudioClip[] footstepSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] specialZombieRoam;
    public float baseResetRoamSound = 11f;
    private AudioSource soundSource;
    public bool canRoamSound = true;

    private Vector3 SlippyPosition;
    public bool hasRagdolled = false;

    private bool isPaused = false;
    Vector3 lastAgentVelocity;
    NavMeshPath lastAgentPath;


    //Lets get a reference to the rigid body b/c the enemy should control
    //their enable disable if we ever wanted to ragdoll and not kill 
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Set target to the player's transform, agent to this instance of an enemy
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.speed = speed;

        soundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Don't do anything if we are frozen
        if (FreezeController.isFrozen)
            return;

        if (!hasRagdolled)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            // Once within the set distance, navigate enemy to player
            if (distance <= detectionRadius)
            {
                //Here is where we set the destination, therefore we need to get their direction, and then just set their destination for that 
                if (slipNSlide)
                {
                    SlipNSlide(); 
                }
                else
                {
                    agent.SetDestination(target.position);
                    agent.speed = speed;
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isSlipping", false);
                    anim.SetBool("inHitDistance", false);
                    if (soundSource.isPlaying == false)
                    {
                        int n = Random.Range(1, footstepSounds.Length);
                        soundSource.clip = footstepSounds[n];
                        soundSource.PlayOneShot(soundSource.clip);

                        footstepSounds[n] = footstepSounds[0];
                        footstepSounds[0] = soundSource.clip;
                    }

                    if (specialZombieRoam.Length > 1 && canRoamSound)
                    {
                        int n = Random.Range(1, specialZombieRoam.Length);
                        soundSource.clip = specialZombieRoam[n];
                        soundSource.PlayOneShot(soundSource.clip, 0.65f);

                        footstepSounds[n] = footstepSounds[0];
                        footstepSounds[0] = soundSource.clip;

                        canRoamSound = false;
                        StartCoroutine(ResetRoamSound(baseResetRoamSound));
                    }
                }


                // When enemy close enough, attack and remain facing the player by rotating if necessary
                if (distance <= agent.stoppingDistance && !slipNSlide)
                {
                    anim.SetBool("isWalking", false);
                    FaceTarget();
                    anim.SetBool("inHitDistance", true);
                    // TODO attack (animation, hit reg, player health, etc.)
                    if (canAttack && !FreezeController.isFrozen)
                        AttackPlayer();
                }
                else
                    isFirstAttack = true;
            }
        }
    }

    //Maybe we could make the tank outright fall down and take a while to get up? 
    public virtual void SlipNSlide()
    {
        SlippyPosition = transform.position + transform.forward * distanceForward;
        //We need to make the target position in a straight line 
        agent.SetDestination(SlippyPosition);
        agent.speed = (speed * .5f) + .25f; 
        anim.SetBool("isWalking", false);
        anim.SetBool("isSlipping", true);
        anim.SetBool("inHitDistance", false);
    }


    //Stealing what Henry did in the weapon controller for their attacks as well 
    IEnumerator ResetAttackCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isAttacking = false;
        canAttack = true;
    }

    //Maybe make all of these methods virtual so they can be overridden for other enemy types
    public void AttackPlayer()
    {
        int n = Random.Range(1, attackSounds.Length);
        soundSource.clip = attackSounds[n];
        soundSource.PlayOneShot(soundSource.clip, 0.65f);

        attackSounds[n] = attackSounds[0];
        attackSounds[0] = soundSource.clip;

        if (isFirstAttack)
        {
            StartCoroutine(ResetAttackCooldown(waitTimeBeforeAttack));
            isFirstAttack = false;
        }
        else
        {
            isAttacking = true;
            canAttack = false;
            // Commented out bc we changed player damage to be hitbox based. See "EnemyToPlayerCollisionDetection.cs"
            //PlayerStatController.instance.PlayerTakesDamage(5);
            StartCoroutine(ResetAttackCooldown(attackCooldown));    
        }
    }

    IEnumerator WaitToDamage()
    {
        yield return new WaitForSeconds(ragdollTime);
        // We only ragdoll on death so save the computation
        // TakeDamage(damage);
        Die();
    }

    IEnumerator ResetRoamSound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime + Random.Range(1.0f, 8.0f));
        canRoamSound = true;
    }

    //This will manage our wetfloor sign 
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "WetFloor")
        {
            //Just flip our bool to change the way update happens 
            //bool x;
            //if
            slipNSlide = true; 
        }
    }

    public void ManualSlipSet(bool request)
    {
        slipNSlide = request; 
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WetFloor")
        {
            //Just flip our bool to change the way update happens 
            slipNSlide = false;
        }
    }

    public void TakeDamage(float damage, bool isItem = false)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
            Die(isItem);
    }

    public bool ShouldRagDoll(float damage)
    {
        if (enemyHealth - damage <= 0)
        {
            hasRagdolled = true;
            rb.isKinematic = false;
            agent.enabled = false;  
            anim.enabled = false;
            //Start coroutine that waits to take damage
            StartCoroutine(WaitToDamage());
            return true; 
        }
        else return false;
    }

    public void Pause()
    {
        if (isPaused) return;

        lastAgentVelocity = agent.velocity;
        lastAgentPath = agent.path;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        anim.SetBool("isWalking", false);
        anim.SetBool("isSlipping", false);
        anim.SetBool("inHitDistance", false);

        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused) return;

        agent.velocity = lastAgentVelocity;
        agent.SetPath(lastAgentPath);

        anim.SetBool("isWalking", true);
        anim.SetBool("isSlipping", false);
        anim.SetBool("inHitDistance", false);

        isPaused = false;
    }

    public void Die(bool isItem = false)
    {
       RandomStatDrop statDrop = GetComponent<RandomStatDrop>();
        if (statDrop != null)
        {
            if (statDrop.maybeDropStat())
            {
                // Play sound letting player know a stat has dropped 
            }

            // Death animation?? 

            Destroy(gameObject);

            // Update the points 
            ScoreManager.instance.IncreaseScore(pointWorth); 

            // Register the stat as dead
            PlayerStatController.instance.numZombies--;
            PlayerStatController.instance.numKills++;
            FreezeController.zombies.Remove(gameObject.GetInstanceID());
        }
        else
            Debug.LogWarning("There was never a random Stat dropped assigned to this enemy"); 
    }

    void FaceTarget()
    {
        // Calculate direction and rotation to target
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smooth the rotation using Slerp: spherically interpolate the rotation over a deltaTime
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Show the enemy's detection radius as a red WireSphere in Unity Editor
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}