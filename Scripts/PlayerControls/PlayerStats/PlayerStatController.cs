using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour
{

    

    //Singleton so we can call else where
    public static PlayerStatController instance;

    //Delegate to notify everywhere 
    public delegate void OnStatChange();
    public OnStatChange onStatChange;


    //Delegate to notify everywhere 
    public delegate void OnRoomDisabled();
    public OnStatChange onRoomDisabled;

    public float regenWaitTime = 5f;

    private bool isPlayerDead = false; 

    GameManger gameManger;

    private bool midRegen = false;
    private bool midUrgeSear = false;

    public Dictionary<int, float> stats = new Dictionary<int, float>();

    //We will use this for time 
    public float timeOfLastClean; 

    public GameObject m_GotHitScreen;
    public float redTransparency = 0.3f;
    public float redFadeRate = 0.001f;

    public GameObject m_GettingFilthySceen;
    public float brownMaxTransparency = .45f;
    public float brownFadeRate = .015f;

    public float incrementFilthColorTime = 1f; 
    private float timeSinceLastChange = 0f;

    public float timeTilDebuf = 30f;
    public bool mustClean = false;
    public float staticBuffTime = 5f;
    public float buffDecayTime = 3f;
    public float damageMultiplier = 1.5f;
    public int speedAddtion = 25;

    private bool shouldBuff = false;
    private bool shouldDecay = false;

    public bool isCleaning = false;

    public float maxZombies = 10;
    public float maxFilth = 5;
    public float zombieInc = 0.75f;
    public float filthInc = 0.5f;
    public int numZombies = 0;
    public int numFilth = 0;

    public int zombieMaxCap = 150;
    public int filthMaxCap = 300;

    public int numKills;
    public int numCleans;

    public int totalRooms = 20;
    public int roomsCleared = 0;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of player stats found!!");
            return;
        }

        instance = this;



        stats[(int)StatGlobals.STAT_ENUM.HEALTH] = 100;
        stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] = stats[(int)StatGlobals.STAT_ENUM.HEALTH];
        stats[(int)StatGlobals.STAT_ENUM.SPEED] = 5;
        stats[(int)StatGlobals.STAT_ENUM.DAMAGE] = 50;
        stats[(int)StatGlobals.STAT_ENUM.REGEN] = 1; 

        //Lets get the game manager to let it know when we've died
        gameManger = gameObject.GetComponent<GameManger>();

        timeOfLastClean = Time.time; 

        numCleans = 0;
        numKills = 0;

        roomsCleared = 0;

    }

    void Update()
    {

        if (FreezeController.isFrozen)
            timeOfLastClean += Time.deltaTime;

        //Regen health
        if (!midRegen && stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] < stats[(int)StatGlobals.STAT_ENUM.HEALTH] && !FreezeController.isFrozen)
        {
            StartCoroutine(WaitForRegen());
        }

        //Constant check if urge full
        if (!FreezeController.isFrozen && Time.time - timeOfLastClean >= timeTilDebuf)
        {
            mustClean = true;
        }

        //Sear em if we gotta 
        if (!FreezeController.isFrozen && mustClean && !midUrgeSear)
        {
            StartCoroutine(UrgePain());
        }

        if (!FreezeController.isFrozen && Time.time - timeOfLastClean <= staticBuffTime)
        {
            shouldBuff = true;
        }
        else if (!FreezeController.isFrozen && Time.time - timeOfLastClean >= staticBuffTime && (Time.time - timeOfLastClean) <= (buffDecayTime + staticBuffTime))
        {
            shouldBuff = false;
            shouldDecay = true; 
        }
        else
        {
            shouldBuff = false;
            shouldDecay = false; 
        }



            //fade out the red layer from getting hit
            if (m_GotHitScreen != null)
        {
            if (m_GotHitScreen.GetComponent<Image>().color.a > 0)
            {
                var color = m_GotHitScreen.GetComponent<Image>().color;
                color.a -= redFadeRate;
                m_GotHitScreen.GetComponent<Image>().color = color;
            }
        }

        if (!FreezeController.isFrozen)
            timeSinceLastChange += Time.deltaTime;

        if (!FreezeController.isFrozen && m_GettingFilthySceen != null && timeSinceLastChange >= incrementFilthColorTime)
        {
            timeSinceLastChange = 0; 
            if (m_GettingFilthySceen.GetComponent<Image>().color.a < brownMaxTransparency)
            {
                var color = m_GettingFilthySceen.GetComponent<Image>().color;
                color.a += brownFadeRate;
                m_GettingFilthySceen.GetComponent<Image>().color = color;
            }
        }




    }

    private IEnumerator WaitForRegen()
    {
        midRegen = true;
        yield return new WaitForSeconds(regenWaitTime);
        HealthRegen();
        midRegen = false; 
    }

    private IEnumerator UrgePain()
    {
        midUrgeSear = true;
        yield return new WaitForSeconds(1f);
        //Calc the amount of pain 
        float pain = (getScaledRegen() / regenWaitTime) + 1;
        //final check to make sure we haven't cleaned a pile in this weight time 
        if (mustClean)
        {
            PlayerTakesDamage(pain);
            flashScreen();
        }
        
        midUrgeSear = false;
    }

    public float getScaledRegen()
    {
        return (1.25f * (stats[(int)StatGlobals.STAT_ENUM.SPEED] / Mathf.Sqrt(stats[(int)StatGlobals.STAT_ENUM.SPEED]))) + 1;
    }


    public float getScaledSpeed()
    {
        //This might be a lil goofy, but lets find out
        if (shouldBuff)
        {
            return ((stats[(int)StatGlobals.STAT_ENUM.SPEED] + speedAddtion) / Mathf.Sqrt(stats[(int)StatGlobals.STAT_ENUM.SPEED] + speedAddtion)) + 3;
        }
        else if (shouldDecay)
        {
            //Lets figure out the new speed addition
            float newSpeedBoost = ((1 - ((Time.time - timeOfLastClean) / (buffDecayTime + staticBuffTime))) * speedAddtion);
            return ((stats[(int)StatGlobals.STAT_ENUM.SPEED]  + newSpeedBoost) / Mathf.Sqrt(stats[(int)StatGlobals.STAT_ENUM.SPEED] + newSpeedBoost)) + 3;
        }
        return  (stats[(int)StatGlobals.STAT_ENUM.SPEED] / Mathf.Sqrt(stats[(int)StatGlobals.STAT_ENUM.SPEED])) + 3;
    }


    private void HealthRegen()
    {
        if (!isPlayerDead)
        {
            if (stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] + getScaledSpeed() > stats[(int)StatGlobals.STAT_ENUM.HEALTH])
            {
                stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] = stats[(int)StatGlobals.STAT_ENUM.HEALTH];
            }
            else
                stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] += getScaledRegen();
        }

    }

    public float GetDamage()
    {
        if (mustClean)
            return stats[(int)StatGlobals.STAT_ENUM.DAMAGE] * 0.1f;
        else if (shouldBuff)
        {
            return stats[(int)StatGlobals.STAT_ENUM.DAMAGE] * damageMultiplier;
        }
        else if (shouldDecay)
        {
            float newDamageMult = (1 - ((Time.time - timeOfLastClean) / (buffDecayTime + staticBuffTime))) * damageMultiplier;
            return stats[(int)StatGlobals.STAT_ENUM.DAMAGE] * (newDamageMult + 1);
        }
        else
            return stats[(int)StatGlobals.STAT_ENUM.DAMAGE]; 


    }


    public void UpdateStat(int statType, float value)
    {
        //Debug.Log("was " + stats[statType]);
        //We want the current health to increase when the max health increases? Could change later: 
        if (statType == (int)StatGlobals.STAT_ENUM.HEALTH)
        {
            stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] += value;
        }
        stats[statType] += value;
        //Debug.Log("now " + stats[statType]);

        if (onStatChange != null)
            onStatChange.Invoke();
        else
            Debug.Log("No one listening for stat changes!");
    }


    public void PlayerTakesDamage(float damage)
    {
        if(!isPlayerDead)
        {
            flashScreen();
            stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] -= damage;

            if (stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] <= 0)
            {
                stats[(int)StatGlobals.STAT_ENUM.CURRENTHEALTH] = 0;
                KillPlayer();
            }
        }
    }

    void flashScreen()
    {
        if (m_GotHitScreen != null)
        {
            var color = m_GotHitScreen.GetComponent<Image>().color;
            color.a = redTransparency;

            m_GotHitScreen.GetComponent<Image>().color = color;
        }
    }


    private void KillPlayer()
    {
        isPlayerDead = true;
        gameManger.RestartGame(); 
    }


    public void setTimeOfLastClean()
    {
        //We'll also reset the transparency
        var color = m_GettingFilthySceen.GetComponent<Image>().color;
        color.a = brownFadeRate;

        mustClean = false;

        m_GettingFilthySceen.GetComponent<Image>().color = color;

        timeOfLastClean = Time.time; 
    }

    public void IncZombieCap()
    {
        maxZombies = Mathf.Min(zombieMaxCap, maxZombies + zombieInc);
    }

    public void IncFilthCap()
    {
        maxFilth = Mathf.Min(filthMaxCap, maxFilth + filthInc);
    }

    public void IncCaps(bool isZombie = false)
    {
        IncZombieCap();
        IncFilthCap();

        if (isZombie)
            numKills++;
        else
            numCleans++;
    }

    public void IncRoomsDisabled()
    {
        roomsCleared++;

        if (onRoomDisabled != null)
            onRoomDisabled.Invoke();
        else
            Debug.LogError("No one listening for rooms disabled"); 

    }
}
