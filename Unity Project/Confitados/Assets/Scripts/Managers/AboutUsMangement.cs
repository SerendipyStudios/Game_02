using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUsMangement : MonoBehaviour
{
public void GoToURL(string url)
    {
        Application.ExternalEval("window.open('" + url + "', '_blank')");
    }
}
