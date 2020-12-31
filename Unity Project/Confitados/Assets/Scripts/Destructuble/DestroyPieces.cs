using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPieces : MonoBehaviour
{
    public void DestroyAllPieces()
    {
        Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}
