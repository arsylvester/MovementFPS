using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float xVelocity = 0;
    [SerializeField] float yVelocity = 1;
    [SerializeField] float zVelocity = 0;
    [SerializeField] bool resetVelcity = true;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.ApplyVelocity(xVelocity, yVelocity, zVelocity, resetVelcity);
        }
    }
}
