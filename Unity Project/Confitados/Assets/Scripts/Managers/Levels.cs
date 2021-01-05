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

    public static LevelsEnum GetNextEnum(LevelsEnum level)
    {
        switch (level)
        {
            case LevelsEnum.Galletown:
                return LevelsEnum.TartaDeTortas;
                break;
            case LevelsEnum.TartaDeTortas:
                return LevelsEnum.Galletown;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
    
    public static LevelsEnum GetPreviousEnum(LevelsEnum level)
    {
        switch (level)
        {
            case LevelsEnum.Galletown:
                return LevelsEnum.TartaDeTortas;
                break;
            case LevelsEnum.TartaDeTortas:
                return LevelsEnum.Galletown;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    public static string GetString(LevelsEnum level)
    {
        switch (level)
        {
            case LevelsEnum.Galletown:
                return "Galletown";
                break;
            case LevelsEnum.TartaDeTortas:
                return "Tarta de tortas";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
