using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalEncounter : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPos;
    [SerializeField] Transform keySpawnPos;
    [SerializeField] Transform keyMovePos;
    [SerializeField] GameObject key;
    [SerializeField] GameObject grassBoss;
    [SerializeField] GameObject sewerBoss;
    [SerializeField] bool useMini;
    [SerializeField] GameObject miniBoss;

    int bossItr;
    int keyItr;

    bool isActivated;

    public void StartEncounter()
    {
        if (!isActivated)
        {
            isActivated = true;
            grassBoss.SetActive(true);
            bossItr = 1;
            keyItr = 0;
        }
    }


    void Update()
    {
        if (grassBoss == null && bossItr == 1)
        {
            sewerBoss.SetActive(true);
            bossItr++;
        }
        else if (sewerBoss == null && bossItr == 2)
        {
            if (useMini)
            {
                miniBoss.SetActive(true);
                bossItr++;
            }
            else
            {
                if (keyItr == 0)
                {
                    spawnKey();
                }
            }
        }
        else if (miniBoss == null && bossItr == 3)
        {
            if (keyItr == 0)
            {
                spawnKey();
            }
        } 
    }

    void spawnKey()
    {
        GameObject spawned = Instantiate(key, keySpawnPos.position, keySpawnPos.rotation);
        spawned.GetComponent<FinalKey>().keyMovePosition = keyMovePos;
        keyItr = 1;
    }
}
