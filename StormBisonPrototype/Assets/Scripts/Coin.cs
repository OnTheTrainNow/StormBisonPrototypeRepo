using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static Coin instance;
    [SerializeField] int coinValue;
    [SerializeField] int healAmount;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag ("Player"))
        {
            gameManager.instance.updateCoinUI(coinValue);
            gameManager.instance.playerScript.HealAmount(healAmount);
            Destroy(gameObject);
        }
    }
}
