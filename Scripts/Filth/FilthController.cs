using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilthController : MonoBehaviour
{
    public int dim;

    public bool autoSpawn;
    public bool autoDestroy;

    public float scaleTime;

    [Range(0, 10)]
    public int smoothing;

    // Dim and respective score boost
    int[] dimThresholds = { 25, 45 };
    int[] scoreValues = { 100, 200, 400 };

    public int startTier, currentTier;
    int rad, radSq;

    bool scaling = false;

    bool[,] filth;

    // Start is called before the first frame update
    void Start()
    {
        if (autoSpawn)
            GenerateFilth();
    }

    /**
     * Getters and setters
     */

    public void SetDim(int _dim)
    {
        dim = _dim;
    }

    public int GetStartTier()
    {
        return startTier;
    }

    public int GetCurrentTier()
    {
        return currentTier;
    }
    
    public bool GetScaling()
    {
        return scaling;
    }

    public void SetScaling(bool newScaling)
    {
        scaling = newScaling;
    }

    /**
     * Scaling
     */

    // Scale self down by half, 
    IEnumerator Scaler(bool destroy)
    {
        SetScaling(true);

        // We need to also scale y for the particles
        // Lerp from start to dest
        Vector3 start = gameObject.transform.localScale;
        Vector3 dest = destroy ? new Vector3(0, 0, 0) : start / 2;

        float startTime = Time.time;
        // Scale us while there is still time left
        while ((Time.time - startTime) < scaleTime)
        {
            // Lerp!
            float timeSpent = Time.time - startTime;
            gameObject.transform.localScale = Vector3.Lerp(start, dest, timeSpent / scaleTime);
            yield return null;
        }

        // Dump it if dead
        if (destroy)
        {
            ScoreManager.instance.IncreaseScore(scoreValues[startTier]);
            //this gives us 5, 10, 20 
            ScoreManager.instance.IncreaseMoney((int)((scoreValues[startTier] * .1f) * .5f)); 
            PlayerStatController.instance.setTimeOfLastClean(); 
            Destroy(gameObject);

            // Register the filth as cleaned
            PlayerStatController.instance.numFilth--;

            // Increase caps
            PlayerStatController.instance.IncCaps();
        }

        SetScaling(false);
    }

    // Expose a helper for no force destroy
    public void TierDown()
    {
        TierDown(false);
    }

    public void TierDown(bool forceDestroy)
    {
        currentTier--;
        StartCoroutine(Scaler(forceDestroy || currentTier < 0));
    }

    /**
     * Logic
     */

    // Initialize and smooth filth
    public void GenerateFilth()
    {
        startTier = 0;
        while (startTier < dimThresholds.Length && dimThresholds[startTier] <= dim)
            startTier++;
        currentTier = startTier;

        // Calc rad
        rad = dim / 2;
        radSq = rad * rad;

        InitializeFilth();
        
        for (int i = 0; i < smoothing; i++)
            SmoothFilth();

        FilthMeshGenerator meshGen = GetComponent<FilthMeshGenerator>();
        meshGen.GenerateMesh(filth, 0.1f);

        if (autoDestroy)
            InvokeRepeating("Destroy", 3.0f, 0f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    // Randomly generate a filth map based on intensity
    void InitializeFilth()
    {
        filth = new bool[dim, dim];

        // Randomly fill in a circle in the center
        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                int dx = x - rad;
                int dy = y - rad;

                // Don't fill if our point is outside the circle
                int dist = dx * dx + dy * dy;
                if (dist > radSq)
                    continue;
                else
                    filth[x, y] = RandomProvider.random.Next(0, radSq) > dist;
            }
        }
    }

    // Smooth the filth using a cellular automata algorithm
    void SmoothFilth()
    {
        // Buffer to prevent bias
        bool[,] buffer = filth;

        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                int neighbors = GetNeighborCount(x, y);
                if (neighbors > 4)
                    buffer[x, y] = true;
                else if (neighbors < 4)
                    buffer[x, y] = false;
            }
        }

        filth = buffer;
    }

    // Return the number of neighbors in a 3x3 grid
    int GetNeighborCount(int x, int y)
    {
        int neighbors = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx < 0 || nx >= dim || ny < 0 || ny >= dim)
                    continue;
                if (filth[nx, ny])
                    neighbors++;
            }
        }

        return neighbors;
    }

    // Tier down on Mop collision
    private void OnTriggerStay(Collider collider)
    {
        if (!scaling && collider.tag == "Mop" && PlayerStatController.instance.isCleaning)
        {
            TierDown();
        }
    }
}
