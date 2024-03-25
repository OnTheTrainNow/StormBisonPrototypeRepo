using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static Coin instance;
    [SerializeField] int coinValue;
    [SerializeField] int healAmount;

    [SerializeField]AudioSource coinAudioSource; 
    [SerializeField]AudioClip coinClip;

    bool collected;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag ("Player") && !collected)
        {
            collected = true;
            gameManager.instance.updateCoinUI(coinValue);
            gameManager.instance.playerScript.HealAmount(healAmount);
            GetComponent<MeshRenderer>().enabled = false;
            coinAudioSource.PlayOneShot(coinClip, .6f);
            Destroy(gameObject, 1f);
        }
    }
}
