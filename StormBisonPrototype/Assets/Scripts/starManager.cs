using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starManager : MonoBehaviour , IPersistence //most of this functionality couldve been included in gameManager, but I thought it would be better for it to be de-coupled
{
    public static starManager instance; //this will be a singleton like the game Manager

    public List<Transform> starPositions = new List<Transform>(); //the main functionality of this manager is to track positions for stars to move towards

    public bool[] starTracker = new bool[30]; //this bool tracks which stars are collected by the player

    MeshRenderer render;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        if (render != null ) //turn the renderer off for the manager
        {
            render.enabled = false;
        }
    }

    public void SavePersistentData(ref PlayerSaveData saveData)
    {
        starTracker.CopyTo(saveData.playerStars, 0);
    }

    public void LoadPersistentData(PlayerSaveData saveData)
    {
        saveData.playerStars.CopyTo(starTracker, 0);
    }
}
