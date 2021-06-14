using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : ProjectileWeapon
{
    [SerializeField] float throwSpeed = 1;
    [SerializeField] float lifeTime = 1;
    [SerializeField] GameObject projectileToSpawn;
    private GameObject kunaiProjectile;
    private Renderer[] rends;

    protected override void Start()
    {
        base.Start();
        rends = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        //If kunai not thrown after some time show it again.
        if (!(rends[0].enabled) && coolDown + currentCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
        }
    }

    //If not on cooldown spawns a projectile and sets its renderer off.
    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            base.UseWeapon();
            kunaiProjectile = Instantiate(projectileToSpawn, playerCamera.position, transform.rotation);
            kunaiProjectile.GetComponent<KunaiProjectile>().SetParameters(throwSpeed, damage, ui);
            Destroy(kunaiProjectile, lifeTime);
            currentCoolDown = Time.time;
            ToggleAllRenderers(false);

            if (isHit)
            {
                kunaiProjectile.transform.LookAt(hit.point);
            }
        }
    }

    public override void UseAltFireWeapon()
    {
        base.UseAltFireWeapon();
    }

    //Hide or show the held weapon.
    private void ToggleAllRenderers(bool enable)
    {
        foreach(Renderer rend in rends)
        {
            rend.enabled = enable;
        }
    }
}
