using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Random = System.Random;

public class LevelInfo : MonoBehaviour
{
    //Instance
    private static LevelInfo Instance;
    
    //World limits
    [Range(-1, -10)]
    public static float worldLimit = -5f;

    //Initial positions
    public GameObject initialPositions_GameObject;
    [SerializeField] private static List<Transform> initialPositions = new List<Transform>();

    private static List<int> freeInitialPositions = new List<int>();

    private void Awake()
    {
        Instance = this;
        
        for(int i = 0; i<initialPositions_GameObject.transform.childCount; i++)
        {
            initialPositions.Add(initialPositions_GameObject.transform.GetChild(i).transform);
            freeInitialPositions.Add(i);
        }

        if (initialPositions.Count < 6)
        {
            throw new Exception("There aren't enough initial positions set. " +
                                "There must be 6 at least to be playable in the final version.");
        }
    }

    public static LevelInfo GetInstance()
    {
        return Instance;
    }

    //Server side only
    public static Transform GetInitPos(int index)
    {
        return initialPositions[index];
    }

    //Server side only
    public Transform GetRandomFreeInitPos()
    {
        int index = new Random().Next(freeInitialPositions.Count);
        freeInitialPositions.Remove(index);
        StartCoroutine(InitPosCooldown(index));

        return initialPositions[index];
    }

    private static IEnumerator InitPosCooldown(int index)
    {
        yield return new WaitForSeconds(1);
        freeInitialPositions.Add(index);
    }

    #region GUI
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,(float) 0.5);
        Gizmos.DrawCube(new Vector3(0, worldLimit, 0), new Vector3(100, 1, 100));
    }
    
    #endregion
}
