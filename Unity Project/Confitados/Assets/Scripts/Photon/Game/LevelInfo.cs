using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Photon.Game;
using UnityEngine;
using Random = System.Random;

public class LevelInfo : MonoBehaviour
{
    #region Variables

    //Instance
    public static LevelInfo Instance;

    //World limits
    [Range(-1, -10)] [SerializeField] public float worldLimit = -5f;

    //Initial positions
    [SerializeField] public GameObject initialPositions_GameObject;
    [SerializeField] private static List<Transform> initialPositions = new List<Transform>();
    private static List<int> freeInitialPositions = new List<int>();

    //World pieces
    [SerializeField] private List<GameObject> worldPieces;
    private Stack<GameObject> worldPieces_Stack;
    [SerializeField] private float pieceFallTime = 10;
    [SerializeField] private float pieceFallAdviceTime = 3;
    [SerializeField] private Material fallAdviseMaterial;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        Debug.Log("Awake LevelInfo");
        Instance = this;

        //Restart initPos data structures
        initialPositions = new List<Transform>();
        freeInitialPositions = new List<int>();

        for (int i = 0; i < initialPositions_GameObject.transform.childCount; i++)
        {
            initialPositions.Add(initialPositions_GameObject.transform.GetChild(i).transform);
            freeInitialPositions.Add(i);
        }

        if (initialPositions.Count < 6)
        {
            throw new Exception("There aren't enough initial positions set. " +
                                "There must be 6 at least to be playable in the final version.");
        }

        //Init worldPieces Stack
        worldPieces_Stack = new Stack<GameObject>();
        for (var index = worldPieces.Count - 1; index >= 0; index--)
        {
            worldPieces_Stack.Push(worldPieces[index]);
        }
    }

    #endregion

    #region Getters

    public static LevelInfo GetInstance()
    {
        return Instance;
    }

    //Server side only
    public static Transform GetInitPos(int index)
    {
        return initialPositions[index];
    }

    #endregion

    #region InitPos Methods

    //Server side only
    public Transform GetRandomFreeInitPos()
    {
        //Get a position
        int initialIndex = new Random().Next(freeInitialPositions.Count);
        int index = initialIndex;

        //Check if there's nobody below it
        RaycastHit hitInfo;
        Physics.Raycast(initialPositions[index].position, Vector3.down, out hitInfo, 10);
        while (hitInfo.collider != null && hitInfo.collider.gameObject.GetComponent<PlayerController>() != null)
        {
            index = (index + 1) % freeInitialPositions.Count;
            if (index == initialIndex)
                throw new Exception("Player Respawn at LevelInfo. Inifite loop. There's no valid respawn point.");
            Physics.Raycast(initialPositions[index].position, Vector3.down, out hitInfo, 10);
        }

        //Remove it from the pull and start cooldown routine
        //freeInitialPositions.Remove(index);
        //StartCoroutine(InitPosCooldown(index));

        return initialPositions[index];
    }

    /* Unnecesary due to the new raycast check respawn system
    private static IEnumerator InitPosCooldown(int index)
    {
        yield return new WaitForSeconds(1);
        freeInitialPositions.Add(index);
    }
    */

    #endregion

    #region WorldPieces Methods
    
    public void StartGame()
    {
        StartCoroutine(PieceFallCoroutine());
    }

    private IEnumerator PieceFallCoroutine()
    {
        yield return new WaitForSeconds(pieceFallTime - pieceFallAdviceTime);
        //if (GameManager.Instance.gameState != GameManager.GameStateEnum.Playing) yield break;

        if (worldPieces_Stack.Count != 0)
        {
            //GameObject piece = worldPieces[0];
            //piece.GetComponent<Rigidbody>().useGravity = true;
            //worldPieces.Remove(piece);

            GameObject piece = worldPieces_Stack.Pop();

            //Show advice (misma geometría con y+0.5 parpadeando)
            GameObject clone = Instantiate(piece, piece.transform);
            foreach (var elem in clone.GetComponentsInChildren<Collider>())
            {
                elem.enabled = false;
            }

                //Mat change
            Renderer renderer = clone.GetComponent<Renderer>();
                if(renderer !=null) renderer.material = fallAdviseMaterial;
                
            var renderers = clone.GetComponentsInChildren<Renderer>();
            for (var index = 0; index < renderers.Length; index++)
            {
                renderers[index].material = fallAdviseMaterial;
            }

            clone.transform.position += new Vector3(0, 0.5f, 0);

            yield return new WaitForSeconds(pieceFallAdviceTime);

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints -= RigidbodyConstraints.FreezePositionY;
            
            Destroy(clone);
                
            StartCoroutine(PieceFallCoroutine());
        }
    }

    #endregion

    #region GUI

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, (float) 0.5);
        Gizmos.DrawCube(new Vector3(0, worldLimit, 0), new Vector3(1000, 1, 1000));
    }

    #endregion
}