using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager _screenManager;

    // Start is called before the first frame update
    void Start()
    {
        if(_screenManager)
            Destroy(this);
        else
        {
            _screenManager = this;
        }
    }

    //Methods
    public void GoTo_00_0_LogIn()
    {
        SceneManager.LoadScene("Screen_00_0_LogIn");
    }
    
    public void GoTo_01_0_Menu()
    {
        SceneManager.LoadScene("Screen_01_0_MainMenu");
    }
    
    public void GoTo_01_1_0_Tutorial()
    {
        SceneManager.LoadScene("Screen_01_1_0_Tutorial");
    }
    
    public void GoTo_01_1_1_TutorialGame()
    {
        SceneManager.LoadScene("Screen_01_1_1_TutorialGame");
    }
    
    public void GoTo_01_2_Settings()
    {
        SceneManager.LoadScene("Screen_01_2_Settings");
    }
    
    public void GoTo_01_3_Credits()
    {
        SceneManager.LoadScene("Screen_01_3_credits");
    }
    
    public void GoTo_02_0_Anteroom()
    {
        SceneManager.LoadScene("Screen_02_0_anteroom");
    }
    
    public void GoTo_02_1_Lobby()
    {
        SceneManager.LoadScene("Screen_02_1_lobby");
    }
    
    public void GoTo_02_2_Game()
    {
        SceneManager.LoadScene("Screen_02_2_game");
    }
    
    public void GoTo_02_3_Results()
    {
        SceneManager.LoadScene("Screen_02_3_results");
    }

    public void GoTo_03_0_Shop()
    {
        SceneManager.LoadScene("Screen_03_0_shop");
    }
    
    public void GoTo_03_1_Shop_Treasure()
    {
        SceneManager.LoadScene("Screen_03_1_shop_treasure");
    }
    
    public void GoTo_03_2_Shop_Skins()
    {
        SceneManager.LoadScene("Screen_03_2_shop_skins");
    }
    
    
}
