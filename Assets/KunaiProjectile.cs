using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    private float throwSpeed;
    private bool shouldMove = true;

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

    public void SetSpeed(float speed)
    {
        throwSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit: " + other);
        if(other.tag != "Player")
        {
            shouldMove = false;
        }
    }
}
