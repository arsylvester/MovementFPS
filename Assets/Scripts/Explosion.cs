using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 1;
    [SerializeField] bool resetVelcity = true;
    [SerializeField] float lifetime = 1;
    public float upwardsForce = .5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            //Apply velocity in direction away from center of explosion. Add an extra upwards velcity if wanted for game feel.
            Vector3 explosionDirection = player.transform.position - transform.position;
            float distanceFromCenter = Vector3.Distance(player.transform.position, transform.position);
            explosionDirection = explosionDirection.normalized * explosionForce / distanceFromCenter; //Maybe make it not directly divided by distanc from center. Maybe clamp between min and max distance can add
            explosionDirection.y += upwardsForce;
            if (player)
            {
                player.ApplyVelocity(explosionDirection.x, explosionDirection.y, explosionDirection.z, resetVelcity);
            }
        }
    }

    //Add damage
}
