using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class shopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinCountText;
    public shopItemSO[] shopItemSO;
    public shopItemTemplate[] shopItemTemplate;
    public GameObject[] shopItemTemplateGO;
    public Button[] buyButtons;

    // Start is called before the first frame update
    void Start()
    {
        loadItemPanels();
        removePurchasedItem();
    }

    // Update is called once per frame
    void Update()
    {
        coins = gameManager.instance.coinCount; // gets the coin int from gameManager to set here
        coinUI();
        checkBuyable();
        removePurchasedItem();
        gameManager.instance.coinCount = coins;
    }

    public void coinUI()
    {
        coinCountText.text = "Coins: " + coins.ToString();
    }

    public void loadItemPanels()
    {
        // This will fill the itemTemplates with the information from the SO(items/upgrades), this will also show
        // any extra panels we have even if we dont have and item for them.
        for (int i = 0; i < shopItemSO.Length; i++)
        {
            shopItemTemplate[i].itemNameText.text = shopItemSO[i].itemName;
            shopItemTemplate[i].descriptionText.text = shopItemSO[i].description;
            shopItemTemplate[i].priceText.text = "Coins: " + shopItemSO[i].price.ToString();
            shopItemTemplate[i].itemUpgradeImage.sprite = shopItemSO[i].itemUpgradeSprite;
        }

        // This is will only show the upgrade/item panels that actually have a SO(scriptable objects) for an item
        // say we have 11 panels and only 4 SO(items/upgrades) than it will only set active the 4 panels and keep the rest inactive
        for (int i = 0; i < shopItemSO.Length; i++)
        {
            shopItemTemplateGO[i].SetActive(true);
        }
    }

    public void checkBuyable()
    {
        for (int i = 0; i < shopItemSO.Length; i++)
        {
            if (coins >= shopItemSO[i].price)
            {
                buyButtons[i].interactable = true;
            }
            else
            {
                buyButtons[i].interactable = false;
            }
        }
    }

    public void PurchaseItem(int buttonNum)
    {
        gameManager.instance.coinCount -= shopItemSO[buttonNum].price;
        if (buttonNum == 0)
        {
            gameManager.instance.boughtExtraLife = true;
        }
        if (buttonNum == 1)
        {
            gameManager.instance.boughtMaxHPUpgrade = true;
        }
        if (buttonNum == 2)
        {
            gameManager.instance.boughtWaterCapUpgrade = true;
        }
        if (buttonNum == 3)
        {
            gameManager.instance.boughtWaterRefillEff = true;
        }
        DataManager.instance.savePlayerData();
    }

    public void removePurchasedItem()
    {
        if (gameManager.instance.boughtExtraLife == true)
        {
            shopItemTemplate[0].gameObject.SetActive(false);
        }

        if (gameManager.instance.boughtMaxHPUpgrade == true)
        {
            shopItemTemplate[1].gameObject.SetActive(false);
        }

        if (gameManager.instance.boughtWaterCapUpgrade == true)
        {
            shopItemTemplate[2].gameObject.SetActive(false);
        }

        if (gameManager.instance.boughtWaterRefillEff == true)
        {
            shopItemTemplate[3].gameObject.SetActive(false);
        }
    }
}
