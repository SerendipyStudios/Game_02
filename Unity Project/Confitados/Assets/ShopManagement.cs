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
        buySkinsButtons[0].onClick.AddListener(() => BuySkin(200, 6, "Cookies"));
        buySkinsButtons[1].onClick.AddListener(() => BuySkin(400, 7, "Cookies"));
        buySkinsButtons[2].onClick.AddListener(() => BuySkin(500, 8, "Cookies"));
        buySkinsButtons[3].onClick.AddListener(() => BuySkin(5, 9, "Chocolates"));
        buySkinsButtons[4].onClick.AddListener(() => BuySkin(10, 10, "Chocolates"));
    }

    public void BuySkin(int price, int unlockIndex, string type)
    {
        PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt(type) >= price)
        {
            PlayerPrefs.SetInt(type, PlayerPrefs.GetInt(type) - price);
            PlayerPrefs.SetInt("PaidSkin_" + unlockIndex.ToString(), 1); //Unlock skin
            Debug.Log("SKIN " + "PaidSkin_" + unlockIndex + " UNLOCKED!");
       }
    }
}
