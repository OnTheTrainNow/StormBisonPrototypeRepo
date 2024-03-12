using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistence //any script that needs to handle persistent data (Loading/Saving) can inherit from this interface
{
    void LoadPersistentData(PlayerSaveData saveData); //this method is what the script should do with the loaded save data
    void SavePersistentData(ref PlayerSaveData saveData); //this method is what the script should be saving off to the passed in save data
}
