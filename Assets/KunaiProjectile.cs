using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    private float throwSpeed;
    private bool shouldMove = true;
    private int damageToDeal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            transform.Translate(transform.forward * throwSpeed * Time.deltaTime, Space.World);
        }
    }

    public void SetParameters(float speed, int damage)
    {
        throwSpeed = speed;
        damageToDeal = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit: " + other);
        if(other.tag != "Player")
        {
            shouldMove = false;
            if(other.GetComponent<IDamageable>() != null)
            {
                other.GetComponent<IDamageable>().TakeDamage(damageToDeal);
            }
        }
    }
}
