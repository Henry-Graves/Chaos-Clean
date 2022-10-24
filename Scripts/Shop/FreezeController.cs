using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeController
{
    public static Dictionary<int, GameObject> zombies = new Dictionary<int, GameObject>();
    public static bool isFrozen = false;
    


    // Freeze the game
    public static void FreezeGame()
    {
        // We're gonna have to freeze urge 



        isFrozen = true;
        foreach (KeyValuePair<int, GameObject> entry in zombies)
        {
            EnemyController ec = entry.Value.GetComponent<EnemyController>();
            if (ec != null)
                ec.Pause();
        }
    }

    // UnFreeze the game
    public static void UnFreezeGame()
    {
        isFrozen = false;
        foreach (KeyValuePair<int, GameObject> entry in zombies)
        {
            EnemyController ec = entry.Value.GetComponent<EnemyController>();
            ec.Resume();
        }

    }
}
