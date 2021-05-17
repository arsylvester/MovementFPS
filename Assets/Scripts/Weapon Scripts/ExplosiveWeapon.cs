using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveWeapon : ProjectileWeapon
{
    [SerializeField] float throwSpeed = 1;
    [SerializeField] float lifeTime = 1;
    [SerializeField] GameObject projectileToSpawn;
    private GameObject projectile;
    private Renderer[] rends;

    protected override void Start()
    {
        base.Start();
        rends = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!(rends[0].enabled) && coolDown + currentCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
        }
    }

    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            base.UseWeapon();
            projectile = Instantiate(projectileToSpawn, playerCamera.position, transform.rotation);
            projectile.GetComponent<ExplosiveProjectile>().SetParameters(throwSpeed, damage, ui);
            Destroy(projectile, lifeTime);
            currentCoolDown = Time.time;
            ToggleAllRenderers(false);

            if (isHit)
            {
                projectile.transform.LookAt(hit.point);
            }
        }
    }

    public override void UseAltFireWeapon()
    {
        base.UseAltFireWeapon();
    }

    private void ToggleAllRenderers(bool enable)
    {
        foreach (Renderer rend in rends)
        {
            rend.enabled = enable;
        }
    }
}
