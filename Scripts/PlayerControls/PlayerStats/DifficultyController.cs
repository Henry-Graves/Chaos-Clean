using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    public int scaleSpeed = 1;
    int startingDistThreshold;
    IEnumerator increaseDifficulty()
    {
        for (int newThreshold = startingDistThreshold; newThreshold >= 0; newThreshold -= scaleSpeed)
        {
            FilthSpawnableFloorController.distThreshold = newThreshold;
            yield return new WaitForSeconds(5);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingDistThreshold = FilthSpawnableFloorController.distThreshold;
        StartCoroutine(increaseDifficulty());
    }
}
