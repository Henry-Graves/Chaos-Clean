using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to keep track of a reference to the player character.
// This is faster than searching for our player using FindGameObjectWithTag, which searches through all GameObjects in the scene
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
    }

    public GameObject player;
}
