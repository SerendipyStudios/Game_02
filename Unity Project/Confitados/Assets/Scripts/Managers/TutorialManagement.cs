using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManagement : MonoBehaviour
{
    public Sprite mobileControls;
    public Sprite pcControls;

    private void Start()
    {
        GetComponent<Image>().sprite = SystemInfo.operatingSystem.Contains("Android") ||
            SystemInfo.operatingSystem.Contains("iPhone") ? 
            mobileControls : pcControls;
    }
}
