using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [SerializeField] AudioSource musicSource;
    [SerializeField] List<AudioClip> MusicList;

    float OriginalVolume;
    string sceneName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        OriginalVolume = musicSource.volume;
        sceneName = "";
    }


    public void loadMusic()
    {
        if (sceneName == SceneManager.GetActiveScene().name) { return; } //if this scene is already loaded than don't change the music

        sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "MainLobby":
                {
                    musicSource.clip = MusicList[0];
                    break;
                }
            case "GrassLevel":
                {
                    musicSource.clip = MusicList[1];
                    break;
                }
            case "SewerLevel":
                {
                    musicSource.clip = MusicList[2];
                    break;
                }
            case "EnemyHQLevel":
                {
                    musicSource.clip = MusicList[3];
                    break;
                }
            case "MiniLevel1":
                {
                    musicSource.clip = MusicList[4];
                    break;
                }
            case "MiniLevel2":
                {
                    musicSource.clip = MusicList[5];
                    break;
                }
            default:
                {
                    musicSource.clip = MusicList[0];
                    break;
                }
        }

        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.isPaused)
        {
            musicSource.volume = 0;
        }
        else
        {
            musicSource.volume = OriginalVolume;
        }
    }
}
