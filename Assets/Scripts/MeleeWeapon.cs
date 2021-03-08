using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //[SerializeField] protected bool hitScan;
    [SerializeField] float meleeRange = 1;
    [SerializeField] ParticleSystem StrikeFVX;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject enemyHitParticle;

    protected RaycastHit hit;
    protected bool isHit;

    protected override void Start()
    {
        base.Start();
    }

    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            StrikeFVX.Play();
            currentCoolDown = Time.time;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, meleeRange))
            {
                print("Melee hit : " + hit.transform);
                isHit = true;
                //Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
                if (hit.transform.GetComponent<IDamageable>() != null)
                {
                    hit.transform.GetComponent<IDamageable>().TakeDamage(damage);
                    ui.ShowHitMarker();
                    Instantiate(enemyHitParticle, hit.point, transform.rotation);
                }
                else
                {
                    Instantiate(hitParticle, hit.point, transform.rotation);
                }
            }
            else
            {
                isHit = false;
            }
        }
    }

    public override void UseAltFireWeapon()
    {

    }

    public override void AltFireWeaponRelease()
    {
        //StartCoroutine(Zoom(zoomAmount, oldZoom));
    }

}
