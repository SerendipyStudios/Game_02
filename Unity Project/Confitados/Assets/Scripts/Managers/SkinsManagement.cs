using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinsManagement : MonoBehaviour
{
    public GameObject[] playerSkins;
    public Sprite[] playerSkinsPreviews;
    public Image playerSkinImage;

    public int skinIndex = 0; //[HERE] this index must be sent to game manager (How?)

    private void Start()
    {
        UpdateSkinPreview();
    }

    public void NextSkin()
    {
        skinIndex = ++skinIndex < playerSkins.Length ? skinIndex++ : 0;
        if (skinIndex > 5)
            NextAvailableSkin();
        UpdateSkinPreview();
    }

    public void PreviousSkin()
    {
        skinIndex = --skinIndex >= 0 ? skinIndex-- : playerSkins.Length - 1;
        if (skinIndex > 5)
            PreviousAvailableSkin();
        UpdateSkinPreview();
    }

    public void UpdateSkinPreview()
    {
        playerSkinImage.sprite = playerSkinsPreviews[skinIndex];
    }

    private void NextAvailableSkin()
    {
        while(PlayerPrefs.GetInt("PaidSkin_" + skinIndex, 0) == 0 && skinIndex > 0)
            skinIndex = ++skinIndex < playerSkins.Length ? skinIndex++ : 0;
    }

    private void PreviousAvailableSkin()
    {
        while (PlayerPrefs.GetInt("PaidSkin_" + skinIndex, 0) == 0 && skinIndex > 5)
            skinIndex = --skinIndex >= 0 ? skinIndex-- : 5;
    }
}
