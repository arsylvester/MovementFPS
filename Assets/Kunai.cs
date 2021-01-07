using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : ProjectileWeapon
{
    [SerializeField] float throwSpeed = 1;
    [SerializeField] float lifeTime = 1;
    [SerializeField] GameObject projectileToSpawn;
    private GameObject kunaiProjectile;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if(kunaiProjectile != null)
        {
            //kunaiProjectile.transform.Translate(kunaiProjectile.transform.forward * throwSpeed * Time.deltaTime, Space.World);
        }
    }

    public override void UseWeapon()
    {
        base.UseWeapon();
        kunaiProjectile = Instantiate(projectileToSpawn, transform.position, transform.rotation);
        kunaiProjectile.GetComponent<KunaiProjectile>().SetSpeed(throwSpeed);
        Destroy(kunaiProjectile, lifeTime);

        if(isHit)
        {
            kunaiProjectile.transform.LookAt(hit.point);
        }
    }
}
