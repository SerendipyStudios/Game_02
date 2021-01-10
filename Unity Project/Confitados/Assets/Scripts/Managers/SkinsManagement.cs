using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinsManagement : MonoBehaviour
{
    public GameObject[] playerSkins;
    public Sprite[] playerSkinsPreviews;
    public Image playerSkinImage;

    [HideInInspector] public int skinIndex = 0; //[HERE] this index must be sent to game manager (How?)

    private void Start()
    {
        UpdateSkinPreview();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            skinIndex = ++skinIndex < playerSkins.Length ? skinIndex++ : 0;
            UpdateSkinPreview();
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Instantiate(playerSkins[skinIndex], new Vector3(7.729721f, 0.21f, 1f), Quaternion.identity, transform);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            skinIndex = --skinIndex >= 0 ? skinIndex-- : playerSkins.Length - 1;
            UpdateSkinPreview();
        }
    }

    public void UpdateSkinPreview()
    {
        playerSkinImage.sprite = playerSkinsPreviews[skinIndex];
    }
}
