using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomProvider
{
    private static string seed = Time.time.ToString();
    public static System.Random random = new System.Random(seed.GetHashCode());
}
