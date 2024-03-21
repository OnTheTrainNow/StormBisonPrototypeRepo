using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //this class holds the data which needs to be saved / loaded from a file
public class PlayerSaveData 
{
    public bool[] playerStars; //this array of bools tracks the players current collected stars (the star manager handles the setting of the bools)
    public int starCount;
    public bool gotFinalKey;
    public bool boughtMaxHPUpgrade;
    public bool boughtWaterCapUpgrade;
    public bool boughtWaterRefillEff;
    public bool boughtExtraLife;

    public PlayerSaveData () 
    {
        playerStars = new bool[30]; //the array size here should match that of the star manager since the two will save and load from each other
        starCount = 0;
        gotFinalKey = false;
        boughtMaxHPUpgrade = false;
        boughtWaterCapUpgrade = false;
        boughtWaterRefillEff = false;
        boughtExtraLife = false;
    }
}
