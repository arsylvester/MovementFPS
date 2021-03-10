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
    [SerializeField] Renderer[] swordRenderers;

    protected RaycastHit hit;
    protected bool isHit;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!(swordRenderers[0].enabled) && coolDown + currentCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
        }
    }

    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            StrikeFVX.Play();
            currentCoolDown = Time.time;
            ToggleAllRenderers(false);
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

    private void ToggleAllRenderers(bool enable)
    {
        foreach (Renderer rend in swordRenderers)
        {
            rend.enabled = enable;
        }
    }

}
