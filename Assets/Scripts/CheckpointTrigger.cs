using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] Transform checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            CheckpointSystem.instance.SetCurrentCheckpoint(checkpoint);
        }
    }
}
