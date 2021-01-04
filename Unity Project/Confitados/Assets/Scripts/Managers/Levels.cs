using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public enum LevelsEnum
    {
        Galletown = 0,
        TartaDeTortas = 1,
    }

    public static string GetScene(LevelsEnum level)
    {
        switch (level)
        {
            case LevelsEnum.Galletown:
                return "Screen_02_2_game_0_galletown";
                break;
            case LevelsEnum.TartaDeTortas:
                return "Screen_02_2_game_1_tartaDeTortas";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
