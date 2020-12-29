﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    //State enum
    public enum BrokenState { full, cracked, veryCracked, broken }
    public BrokenState state = BrokenState.full;

    //Force threshold
    public float threshold;

    //Child models
    public GameObject childModelFull;
    private GameObject childModelCracked;
    private GameObject childModelVeryCracked;

    //Models
    public GameObject crackedVersion;
    public GameObject veryCrackedVersion;
    public GameObject brokenVersion;

    //Colission
    private bool colided;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.tag.CompareTo("Player") == 0 && 
            !colided &&
            (Mathf.Abs(collision.rigidbody.velocity.x) > threshold ||
            Mathf.Abs(collision.rigidbody.velocity.z) > threshold))
        {
            colided = true;
            switch (state)
            {
                case BrokenState.full:
                    state = BrokenState.cracked;
                    childModelCracked = InstantiateNewModel(crackedVersion, childModelFull, transform);
                    Destroy(childModelFull);
                    StartCoroutine(CanBreakAgain());
                    break;
                case BrokenState.cracked:
                    state = BrokenState.veryCracked;
                    childModelVeryCracked = InstantiateNewModel(veryCrackedVersion, childModelCracked, transform);
                    Destroy(childModelCracked);
                    StartCoroutine(CanBreakAgain());
                    break;
                case BrokenState.veryCracked:
                    state = BrokenState.broken;
                    InstantiateNewModel(brokenVersion, childModelVeryCracked, transform);
                    Destroy(childModelVeryCracked);
                    break;
            }
        }
    }

    private GameObject InstantiateNewModel(GameObject original, GameObject referenceModel, Transform parent)
    {
        GameObject newChild;
        newChild = Instantiate(original, referenceModel.transform.position, original.transform.rotation, parent);
        newChild.transform.localScale = referenceModel.transform.localScale;
        return newChild;
    }

    IEnumerator CanBreakAgain()
    {
        yield return new WaitForSeconds(0.5f);
        colided = false;
    }
}