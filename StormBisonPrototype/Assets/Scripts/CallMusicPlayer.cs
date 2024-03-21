using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (MusicPlayer.Instance != null)
        {
            MusicPlayer.Instance.loadMusic();
        }
    }
}
