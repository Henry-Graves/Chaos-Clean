using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilthEnemySpawner : MonoBehaviour
{
    [Range(0, 100)]
    public double spawnChancePerTick = 1;

    //All of our gameobjects moved to these lists 
    public WeightedRandomList<GameObject> tier1Spawns;
    public WeightedRandomList<GameObject> tier2Spawns;
    public WeightedRandomList<GameObject> tier3Spawns;

    void SetSpawnChance(double chance)
    {
        spawnChancePerTick = chance;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Lets subscribe to the controller to know tier changes
        StartCoroutine(SpawnTick());
    }

    IEnumerator SpawnTick()
    {
        while (true)
        {
            if (PlayerStatController.instance.roomsCleared == PlayerStatController.instance.totalRooms)
                break;

            float delay = 1f - Mathf.Max(0f, PlayerStatController.instance.roomsCleared - 7) * 0.06f;

            if (PlayerStatController.instance.roomsCleared >= 15) SetSpawnChance(1.5);
            if (PlayerStatController.instance.roomsCleared >= 16) SetSpawnChance(2);
            if (PlayerStatController.instance.roomsCleared >= 17) SetSpawnChance(2.5);
            if (PlayerStatController.instance.roomsCleared >= 18) SetSpawnChance(3.5);
            if (PlayerStatController.instance.roomsCleared >= 19) SetSpawnChance(5);

            yield return new WaitForSeconds(delay);
            MaybeSpawnEnemy();
        }
    }

    void MaybeSpawnEnemy()
    {
        // Don't spawn zombies if the game is frozen
        if (FreezeController.isFrozen)
            return;

        // Don't spawn zombies if we hit our cap
        if (PlayerStatController.instance.numZombies >= PlayerStatController.instance.maxZombies)
            return;

        double chance = RandomProvider.random.NextDouble() * 100;
        if (chance <= spawnChancePerTick)
        {
            GameObject zombie;
            if (gameObject.GetComponent<FilthController>().GetCurrentTier() == 0)
            {
                //we only spawn base enemy type, but make list if we want to change in future 
                zombie = Instantiate(tier1Spawns.GetRandom(), gameObject.transform.position, Quaternion.identity);
            }
            else if (gameObject.GetComponent<FilthController>().GetCurrentTier() == 1)
            {
                //Use this weighted list
                zombie = Instantiate(tier2Spawns.GetRandom(), gameObject.transform.position, Quaternion.identity);
            }
            else
            {
                //Use this weighted list 
                zombie = Instantiate(tier3Spawns.GetRandom(), gameObject.transform.position, Quaternion.identity);
            }

            // Increment zombies, and register
            PlayerStatController.instance.numZombies++;
            FreezeController.zombies.Add(zombie.GetInstanceID(), zombie);
        }
    }
}
