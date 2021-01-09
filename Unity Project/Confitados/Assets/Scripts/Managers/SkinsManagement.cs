using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsManagement : MonoBehaviour
{
    public GameObject[] playerSkins;

    public int skinIndex = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            skinIndex = ++skinIndex < playerSkins.Length ? skinIndex++ : 0;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Instantiate(playerSkins[skinIndex], new Vector3(7.729721f, 0.21f, 1f), Quaternion.identity, transform);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            skinIndex = --skinIndex >= 0 ? skinIndex-- : playerSkins.Length - 1;
        }
    }
}
