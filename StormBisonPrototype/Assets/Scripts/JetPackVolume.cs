using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackVolume : MonoBehaviour
{
    [SerializeField] GameObject jetPackFX;
    [SerializeField] AudioSource jetPackSoundSource;

    float jetPackDefaultVol;
    // Start is called before the first frame update
    void Start()
    {
        jetPackDefaultVol = jetPackSoundSource.volume; 
    }

    // Update is called once per frame
    void Update()
    {
        if (jetPackSoundSource.enabled)
        {
            if (gameManager.instance.isPaused)//if the game is paused and the player is jetpacking turn the volume to zero
            {
                jetPackSoundSource.mute = true;
            }
            else //otherwise set the volume to the default value
            {
                jetPackSoundSource.mute = false;
            }
        }
    }
}
