using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveFileTriggerTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DataManager.instance.forceNewGameFile();
        }
    }
}
