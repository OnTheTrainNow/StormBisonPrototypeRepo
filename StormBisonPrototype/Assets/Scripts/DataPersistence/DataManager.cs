using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //this is used in getSaveDataList to find objects using the persistence interface (you can't search for Interfaces normally without this)
using UnityEngine.Windows;
using System.IO;

public class DataManager : MonoBehaviour
{
    [SerializeField] string fileName;

    public static DataManager instance; //the data manager is a singleton that goes onto the scene

    PlayerSaveData saveData; //this is the actual save data object used for all scripts 
    List<IPersistence> hasSaveData; //this list will be used to find all objects in the scene that inherit from the persistence interface

    SaveFileHandler saveFileHandler; //the save file handler class has the functionality for reading/writing the save data to a file

    private void Awake()
    {
        instance = this; //there should be only one data manager in a scene
    }

    public void Start()
    {
        saveFileHandler = new SaveFileHandler(fileName); //create a fileHandler
        getSaveDataList(); //get the list of IPersistence objects
        loadPlayerData(); //load the players save data
    }

    public void getSaveDataList() //this method gets all the objects in the scene that inherit from IPersistence
    {
        hasSaveData = FindObjectsOfType<MonoBehaviour>().OfType<IPersistence>().ToList(); //find all the IPersistence objects in the save and save them to the list
    }

    public void createNewSaveFile() //this method creates a save data if one doesnt exist already
    {
        if (saveData == null) //if the save file doesnt exist
        {
            saveData = new PlayerSaveData(); //create one
        }
    }

    public void savePlayerData() //this method calls the save methods of each IPersistence object
    {
        foreach (IPersistence pesistentDataObject in hasSaveData) //iterate over each IPersistence object
        {
            pesistentDataObject.SavePersistentData(ref saveData); //call their save method (it changes the save data by reference)
        }

        saveFileHandler.writeData(saveData); //tell the file handler to write the save data to a file
    }

    public void loadPlayerData() //this method calls the load methods for each of the IPersistence objects
    {
        saveData = saveFileHandler.readData(); //tell the file handler to read the save data from the file
        createNewSaveFile(); //this method checks if saveData exists or not 

        foreach (IPersistence pesistentDataObject in hasSaveData) //iterate over each IPersistence object
        {
            pesistentDataObject.LoadPersistentData(saveData); //call their load method passing in the save data as a parameter
        }
    }

    public void forceNewGameFile()
    {
        string fileDirectoryPath = Application.persistentDataPath; //set the directory path to the persistent data path of the application
        string fullFilePath = Path.Combine(fileDirectoryPath, fileName); //combine the two strings above to get the full file path
        
        if (System.IO.File.Exists(fullFilePath))
        {
            System.IO.File.Delete(fullFilePath);
        }
        createNewSaveFile();
    }
}
