using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //[SerializeField] protected bool hitScan;
    [SerializeField] float meleeRange = 1;
    [SerializeField] float holsterCoolDown = .5f;
    [SerializeField] ParticleSystem[] StrikeFVXs;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject enemyHitParticle;
    [SerializeField] Renderer[] swordRenderers;

    protected RaycastHit hit;
    protected bool isHit;

    private int strikeVFXIndex;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!(swordRenderers[0].enabled) && coolDown + holsterCoolDown + currentCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
            strikeVFXIndex = 0;
        }
    }

    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            StrikeFVXs[strikeVFXIndex].Play();
            if(++strikeVFXIndex >= StrikeFVXs.Length)
            {
                strikeVFXIndex = 0;
            }

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
