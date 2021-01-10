using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EconomyManagement : MonoBehaviour
{
    public Button[] exchangeButtons;
    public Image[] buyButtons;
    public Button nextButton;
    public Button backButton;

    private void Start()
    {
        //On click events for all buttons
        exchangeButtons[0].onClick.AddListener(() => CookiesToChocolates(300, 1));
        exchangeButtons[1].onClick.AddListener(() => CookiesToChocolates(800, 3));
        exchangeButtons[2].onClick.AddListener(() => CookiesToChocolates(2500, 10));

        exchangeButtons[3].onClick.AddListener(() => ChocolatesToChookies(1, 50));
        exchangeButtons[4].onClick.AddListener(() => ChocolatesToChookies(3, 200));
        exchangeButtons[5].onClick.AddListener(() => ChocolatesToChookies(10, 750));
    }

    public void CookiesToChocolates(int cookies, int chocolatesChange)
    {
        if(PlayerPrefs.GetInt("Cookies") >= cookies)
        {
            PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies") - cookies);
            PlayerPrefs.SetInt("Chocolates", PlayerPrefs.GetInt("Chocolates") + chocolatesChange);
        }
    }
    public void ChocolatesToChookies(int chocolates, int cookiesChange)
    {
        if (PlayerPrefs.GetInt("Chocolates") >= chocolates)
        {
            PlayerPrefs.SetInt("Chocolates", PlayerPrefs.GetInt("Chocolates") - chocolates);
            PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies") + cookiesChange);
        }
    }

    public void ChangeDisplayedButtons(bool b)
    {
        for(int i = 0; i < exchangeButtons.Length; i++)
            exchangeButtons[i].gameObject.SetActive(!b);
        for (int i = 0; i < buyButtons.Length; i++)
            buyButtons[i].gameObject.SetActive(b);
        nextButton.gameObject.SetActive(!b);
        backButton.gameObject.SetActive(b);
    }
}
