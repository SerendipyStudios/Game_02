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
        TartaDeCastanas = 2,
    }

    public static string GetScene(LevelsEnum level)
    {
        switch (level)//
        {
            case LevelsEnum.Galletown:
                return "Screen_02_2_game_0_galletown";
            case LevelsEnum.TartaDeTortas:
                return "Screen_02_2_game_1_tartaDeTortas";
            case LevelsEnum.TartaDeCastanas:
                return "Screen_02_2_game_2_tartaDeCastanas";
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
            case LevelsEnum.TartaDeTortas:
                return LevelsEnum.TartaDeCastanas;
            case LevelsEnum.TartaDeCastanas:
                return LevelsEnum.Galletown;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
    
    public static LevelsEnum GetPreviousEnum(LevelsEnum level)
    {
        switch (level)
        {
            case LevelsEnum.Galletown:
                return LevelsEnum.TartaDeCastanas;
            case LevelsEnum.TartaDeTortas:
                return LevelsEnum.Galletown;
            case LevelsEnum.TartaDeCastanas:
                return LevelsEnum.TartaDeTortas;
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
            case LevelsEnum.TartaDeTortas:
                return "Tarta de tortas";
            case LevelsEnum.TartaDeCastanas:
                return "Tarta de castañas";
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
