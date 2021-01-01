using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlUI : MonoBehaviour
{
    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    public Joystick joystick;
    public Button dashButton;
    public Button superDashButton;
}
