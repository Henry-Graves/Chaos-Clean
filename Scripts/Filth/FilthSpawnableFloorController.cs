using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilthSpawnableFloorController : MonoBehaviour
{
    // GameObject for the player
    public GameObject playerObject;

    // Filth object to spawn in
    public GameObject filthPrefab;

    public GameObject sprinkler;

    // Random chance modifier based on distance
    public static int distThreshold = 1200;

    // Whether or not we should spawn additional filth
    bool disabled = false;

    bool isRenderDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        StartCoroutine(SpawnTick());

        Vector3 pos = transform.position;
        Vector3 objectSize = Vector3.Scale(transform.lossyScale, GetComponent<MeshFilter>().mesh.bounds.size);
        float xHalf = objectSize.x / 2;
        float zHalf = objectSize.z / 2;
        GenFilthInRange(pos.x - xHalf, pos.x + xHalf, pos.z - zHalf, pos.z + zHalf, 1f);
    }

    IEnumerator SpawnTick()
    {
        while (true)
        {
            if (PlayerStatController.instance.roomsCleared == PlayerStatController.instance.totalRooms)
                break;

            float delay = 1f - Mathf.Max(0f, PlayerStatController.instance.roomsCleared - 7) * 0.06f;
            yield return new WaitForSeconds(delay);
            MaybeSpawnFilth();
        }
    }

    public bool DisableRoom()
    {
        if (disabled)
            return false;

        disabled = true;
        PlayerStatController.instance.IncRoomsDisabled();

        // Kill all child filth
        List<FilthController> childrenFilthControllers = new List<FilthController>(gameObject.GetComponentsInChildren<FilthController>());
        foreach (FilthController fc in childrenFilthControllers)
            fc.TierDown(true);

        sprinkler.SetActive(true);

        return true;
    }

    // Small chance to spawn filth
    // Higher if the player is further away
    void MaybeSpawnFilth()
    {
        // Don't spawn if the game is frozen
        if (FreezeController.isFrozen)
            return;

        // Don't spawn if we are disabled
        if (disabled)
            return;

        // Don't spawn if we are maxed out
        if (PlayerStatController.instance.numFilth >= PlayerStatController.instance.maxFilth)
            return;

        // Get dist from player
        Vector3 pos = transform.position;
        Vector3 playerPos = playerObject.transform.position;
        float dist = Vector3.Distance(playerPos, pos);

        // More likely not to spawn if close to player
        if (RandomProvider.random.Next(0, distThreshold) >= dist) return;

        Vector3 objectSize = Vector3.Scale(transform.lossyScale, GetComponent<MeshFilter>().mesh.bounds.size);

        // Get dims
        float xHalf = objectSize.x / 2;
        float zHalf = objectSize.z / 2;

        // Spawn
        GenFilthInRange(pos.x - xHalf, pos.x + xHalf, pos.z - zHalf, pos.z + zHalf, dist);
    }

    // Get a random point and dimension, and then spawn the filth
    void GenFilthInRange(float xLower, float xUpper, float zLower, float zUpper, float sizeModifier)
    {
        float randX = (float)(RandomProvider.random.NextDouble() * (xUpper - xLower) + xLower);
        float randZ = (float)(RandomProvider.random.NextDouble() * (zUpper - zLower) + zLower);

        // If the point is not on nav mesh, do not spawn
        Vector3 pos = new Vector3(randX, 0, randZ);
        UnityEngine.AI.NavMeshHit hit;
        if (!UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
            return;

        double errorX = Mathf.Abs(pos.x - hit.position.x) + Mathf.Abs(pos.z - hit.position.z);
        double errorY = Mathf.Abs(pos.y - hit.position.y);
        if (errorX > 0 || errorY >= 0.25) return;

        int randDim = RandomProvider.random.Next(50, 50 + (int)(sizeModifier)) / 2;

        // Make the object
        GameObject scaledFilth = filthPrefab;
        FilthController fg = scaledFilth.GetComponent<FilthController>();
        fg.SetDim(randDim);
        fg.GenerateFilth();
        scaledFilth.transform.position = new Vector3(randX, transform.position.y - 0.00001f, randZ);
        scaledFilth.transform.rotation = Quaternion.identity;

        // Spawn as child
        Instantiate(scaledFilth, transform, true);
        
        // Register the filth as spawned
        PlayerStatController.instance.numFilth++;
    }

    public void showRender()
    {
        if (!isRenderDisplayed && !disabled)
            StartCoroutine(TempShowRenderer(.5f));
    }


    IEnumerator TempShowRenderer(float waitTime)
    {
        isRenderDisplayed = true;
        gameObject.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(waitTime);
        gameObject.GetComponent<Renderer>().enabled = false;
        isRenderDisplayed = false; 
    }
}
