using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagement : MonoBehaviour
{
    public Button[] buySkinsButtons;

    private void Start()
    {
        //Check if the player has that skin
        for (int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.GetInt("PaidSkin_" + (i + 6).ToString()) == 1)
                buySkinsButtons[i].interactable = false;
        }

        //On click events for all buttons
        buySkinsButtons[0].onClick.AddListener(() => BuySkin(200, 0, 6, 0));
        buySkinsButtons[1].onClick.AddListener(() => BuySkin(200, 0, 7, 1));
        buySkinsButtons[2].onClick.AddListener(() => BuySkin(500, 0, 8, 2));
        buySkinsButtons[3].onClick.AddListener(() => BuySkin(500, 0, 9, 3));
        buySkinsButtons[4].onClick.AddListener(() => BuySkin(0, 3, 10, 4));
    }

    public void BuySkin(int priceCookies, int priceChocolates, int unlockIndex, int buttonIndex)
    {
        if (PlayerPrefs.GetInt("Cookies") >= priceCookies && PlayerPrefs.GetInt("Chocolates") >= priceChocolates)
        {
            PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies") - priceCookies);
            PlayerPrefs.SetInt("Chocolates", PlayerPrefs.GetInt("Chocolates") - priceChocolates);
            PlayerPrefs.SetInt("PaidSkin_" + unlockIndex.ToString(), 1); //Unlock skin
            buySkinsButtons[buttonIndex].interactable = false;
       }
    }
}
