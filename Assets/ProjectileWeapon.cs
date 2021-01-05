using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] GameObject objectAtEnd;

    public override void UseWeapon()
    {
        print("This is a projectile shot.");

        RaycastHit hit;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 100))
        {
            print("Projectile hit : " + hit.transform);
            Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
        }
    }
}
