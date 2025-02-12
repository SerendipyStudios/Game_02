﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    public Text playerName;
    public Text cookies;
    public Text chocolates;
    public Text level;
    public Text currentExperience_Txt;
    public Image currentExperience_Fill;
    private void Start()
    {
        playerName.text = PhotonNetwork.NickName;
        cookies.text = PlayerPrefs.GetInt("Cookies", 0).ToString();
        chocolates.text = PlayerPrefs.GetInt("Chocolates", 0).ToString();
        level.text = "Nivel: " + PlayerPrefs.GetInt("Level", 1).ToString();
        currentExperience_Txt.text = PlayerPrefs.GetInt("Experience", 0) + "/200 EXP";
        currentExperience_Fill.fillAmount = (float)PlayerPrefs.GetInt("Experience", 0) / 200f;

        foreach(GameObject b in GameObject.FindGameObjectsWithTag("BasicButton"))
        {
            b.GetComponent<Button>().onClick.AddListener(() => SoundManager.sharedInstance.PlayBasicButton());
        }
    }
}
