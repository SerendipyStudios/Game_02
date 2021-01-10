using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManagement : MonoBehaviour
{

    public void BuySkinCookies(int cookies)
    {
        if(PlayerPrefs.GetInt("Cookies") >= cookies)
        {
            PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies") - cookies);
            //Unlock skin
        }
    }

    public void BuySkinChocolates(int chocolates)
    {
        if (PlayerPrefs.GetInt("Chocolates") >= chocolates)
        {
            PlayerPrefs.SetInt("Chocolates", PlayerPrefs.GetInt("Cookies") - chocolates);
            //Unlock skin
        }
    }
}
