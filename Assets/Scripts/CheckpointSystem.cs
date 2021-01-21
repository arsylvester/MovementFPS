using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem instance;
    private Transform currentCheckpoint;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject); 
        }
        player = FindObjectOfType<PlayerController>();
    }

    public void SetCurrentCheckpoint(Transform newPosition)
    {
        currentCheckpoint = newPosition;
    }

    public void ResetPlayerToCheckpoint()
    {
        player.StopMovement();
        if (currentCheckpoint != null)
        {
            player.transform.position = currentCheckpoint.position;
            player.transform.rotation = currentCheckpoint.rotation;
        }
        else
        {
            player.transform.position = Vector3.zero;
        }
        player.ResumeMovement();
    }
}
