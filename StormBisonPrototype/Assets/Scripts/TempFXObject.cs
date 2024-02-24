using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFXObject : MonoBehaviour
{
    [SerializeField] AudioSource SFX;
    [SerializeField] ParticleSystem VFX;

    //optional list version
    [SerializeField] bool useListForSound;
    [SerializeField] List<AudioClip> soundList;

    [SerializeField] float despawnTime;

    IEnumerator Start()
    {
        if (useListForSound) //if use list for sound is set
        {
            SFX.clip = soundList[Random.Range(0, soundList.Count)]; //get a random sound from the list
        }
        SFX.Play();
        VFX.Play();

        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

}
