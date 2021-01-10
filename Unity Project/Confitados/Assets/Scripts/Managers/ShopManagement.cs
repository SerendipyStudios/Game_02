using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagement : MonoBehaviour
{
    public Button[] buySkinsButtons;

    private void Start()
    {
        //On click events for all buttons
        buySkinsButtons[0].onClick.AddListener(() => BuySkin(200, 0, 6));
        buySkinsButtons[1].onClick.AddListener(() => BuySkin(200, 0, 7));
        buySkinsButtons[2].onClick.AddListener(() => BuySkin(500, 3, 8));
        buySkinsButtons[3].onClick.AddListener(() => BuySkin(500, 3, 9));
        buySkinsButtons[4].onClick.AddListener(() => BuySkin(500, 3, 10));
    }

    public void BuySkin(int priceCookies, int priceChocolates, int unlockIndex)
    {
        PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("Cookies") >= priceCookies && PlayerPrefs.GetInt("Chocolates") >= priceChocolates)
        {
            PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies") - priceCookies);
            PlayerPrefs.SetInt("Chocolates", PlayerPrefs.GetInt("Chocolates") - priceChocolates);
            PlayerPrefs.SetInt("PaidSkin_" + unlockIndex.ToString(), 1); //Unlock skin
            Debug.Log("SKIN " + "PaidSkin_" + unlockIndex + " UNLOCKED!");
       }
    }
}
