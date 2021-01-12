using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldsManagement : MonoBehaviour
{
    public GameObject InputPc;
    public GameObject InputMobile;

    private void Start()
    {
        InputPc.SetActive(SystemInfo.operatingSystem.Contains("Windows") || SystemInfo.operatingSystem.Contains("Mac"));
        InputMobile.SetActive(SystemInfo.operatingSystem.Contains("Android") || SystemInfo.operatingSystem.Contains("iPhone"));
    }
}
