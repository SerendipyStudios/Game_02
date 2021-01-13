using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using UnityEngine;

public class ExitConfirmation_Message : MonoBehaviour
{
    public GameObject exitConfirmation;
    public GameObject darkBackground;

    public void ShowMessage(bool show)
    {
        exitConfirmation.SetActive(show);
        darkBackground.SetActive(show);
    }

    public void LeaveGame()
    {
        GameManager.Instance.LeaveGame();
    }
}
