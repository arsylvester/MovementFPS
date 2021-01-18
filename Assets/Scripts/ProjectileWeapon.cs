using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] GameObject objectAtEnd;
    //[SerializeField] protected bool hitScan;

    protected RaycastHit hit;
    protected bool isHit;

    protected override void Start()
    {
        base.Start();
    }

    public override void UseWeapon()
    {

        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 1000))
        {
            print("Projectile hit : " + hit.transform);
            isHit = true;
            //Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
        }
        else
        {
            isHit = false;
        }
    }
}
