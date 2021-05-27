using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 1;
    public float rbExplosionForce = 100;
    [SerializeField] bool resetVelcity = true;
    [SerializeField] float lifetime = 1;
    [SerializeField] int damage = 5;
    public float upwardsForce = .5f;

    private void Start()
    {
        AkSoundEngine.PostEvent("Explosion", gameObject);
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

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeExplosiveDamage(damage, 1000);
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb)
        {
            rb.AddExplosionForce(rbExplosionForce, transform.position, transform.localScale.x);
        }
    }

    //Add damage
}
