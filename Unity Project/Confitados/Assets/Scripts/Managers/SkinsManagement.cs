using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinsManagement : MonoBehaviour
{
    public GameObject[] playerSkins;
    public GameObject[] playerSkinsPreviews;

    public int skinIndex = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerSkinsPreviews[skinIndex].SetActive(false);
            skinIndex = ++skinIndex < playerSkins.Length ? skinIndex++ : 0;
            playerSkinsPreviews[skinIndex].SetActive(true);
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Instantiate(playerSkins[skinIndex], new Vector3(7.729721f, 0.21f, 1f), Quaternion.identity, transform);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            playerSkinsPreviews[skinIndex].SetActive(false);
            skinIndex = --skinIndex >= 0 ? skinIndex-- : playerSkins.Length - 1;
            playerSkinsPreviews[skinIndex].SetActive(true);
        }
    }

    public void UpdateSkinPreview()
    {
        
    }
}
