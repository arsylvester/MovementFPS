using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayDoor : MonoBehaviour
{
    [SerializeField] bool enterence;
    [SerializeField] Collider oneWayCollider; 

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            if (enterence)
            {
                oneWayCollider.enabled = false;
            }
            else
            {
                oneWayCollider.enabled = true;
            }
        }
    }
}
