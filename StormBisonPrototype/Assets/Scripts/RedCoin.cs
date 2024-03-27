using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoin : MonoBehaviour
{
    bool isCollected;
    // Start is called before the first frame update
    void Start()
    {
        RedCoinManager.instance.UpdateRedCoins(1);
        gameManager.instance.updateRedCoinUI(); //update the red coin UI
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            RedCoinManager.instance.UpdateRedCoins(-1);
            gameManager.instance.updateRedCoinUI();
        }
    }
}
