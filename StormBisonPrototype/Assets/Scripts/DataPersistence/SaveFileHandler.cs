using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveFileHandler : MonoBehaviour
{
    string fileName; //this is the files name
    string fileDirectoryPath; //this is the files directory path
    string fullFilePath; //this is the full path for the file (directory and name combined)

    public SaveFileHandler(string fileName)
    {
        this.fileName = fileName; //get the file name
        fileDirectoryPath = Application.persistentDataPath; //set the directory path to the persistent data path of the application
        fullFilePath = Path.Combine(fileDirectoryPath, fileName); //combine the two strings above to get the full file path
    }

    public void writeData(PlayerSaveData saveData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath)); //create a directory at the full path location if one doesn't already exist

            string writeData = JsonUtility.ToJson(saveData); //convert the saveData to a Json string

            using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Create)) //use a filestream in create mode
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream)) //use a streamwriter to write to the file
                {
                    streamWriter.Write(writeData); //write the json string to the file
                }
            }
        }
        catch (Exception ex) //if the code above failed
        {
            Debug.LogException(ex); //log the error
        }
    }

    public PlayerSaveData readData()
    {
        PlayerSaveData returnData = null; //initialize the player data to be returned

        if (File.Exists(fullFilePath)) //if the file at the specified path exists
        {
            try
            {
                string readData = ""; //initialize an empty string (this will store the searialized json)
                using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read)) //use a filestream in open and read mode
                {
                    using (StreamReader streamReader = new StreamReader(fileStream)) //use a stream reader to read the file
                    { 
                        readData = streamReader.ReadToEnd(); //store the contents of the file in the readData string
                    }
                }

                returnData = JsonUtility.FromJson<PlayerSaveData>(readData); //deserealize the json to a PlayerSaveData object
            }
            catch (Exception ex) //if the code above failed
            {
                Debug.LogException(ex); //log the error
            }
        }
        return returnData; //return the savedata
    }
}
