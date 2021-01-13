using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : MonoBehaviour
{
    private bool buttonReady = false;

    public void PlayBasicButton()
    {
        SoundManager.sharedInstance.PlayBasicButton();
    }

    public void PlayReadyButton()
    {
        if(!buttonReady)
            SoundManager.sharedInstance.PlayReadyButton();
        else
            SoundManager.sharedInstance.PlayNotReadyButton();
        buttonReady = !buttonReady;
    }
}
