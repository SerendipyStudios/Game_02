using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperDashPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.CompareTo("Player") == 0)
        {
            other.GetComponent<PlayerController>().ImproveSuperDash();
            Destroy(this.gameObject);
        }
    }
}
