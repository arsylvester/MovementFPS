using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //[SerializeField] protected bool hitScan;
    [SerializeField] float meleeRange = 1;
    [SerializeField] float holsterCoolDown = .5f;
    [SerializeField] ParticleSystem[] StrikeVFXs;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject enemyHitParticle;
    [SerializeField] Renderer[] swordRenderers;
    //[SerializeField] Collider[] HitBoxes;

    protected RaycastHit hit;
    protected bool isHit;

    private int strikeVFXIndex;
    //private Collider activeHitBox;

    protected override void Start()
    {
        base.Start();
        //activeHitBox = HitBoxes[0];
    }

    private void Update()
    {
        if (!(swordRenderers[0].enabled) && coolDown + holsterCoolDown + currentCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
            //activeHitBox.gameObject.SetActive(false);
            strikeVFXIndex = 0;
        }
    }

    public override void UseWeapon()
    {
        if (coolDown + currentCoolDown < Time.time)
        {
            currentCoolDown = Time.time;
            ToggleAllRenderers(false);
   
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, meleeRange))
            {
                print("Melee hit : " + hit.transform);
                isHit = true;
                //Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
                if (hit.transform.GetComponent<IDamageable>() != null)
                {
                    print(strikeVFXIndex);
                    hit.transform.GetComponent<IDamageable>().TakeDamage(damage, StrikeVFXs[strikeVFXIndex].transform.up);
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

            StrikeVFXs[strikeVFXIndex].Play();
            //activeHitBox.gameObject.SetActive(false);
            // activeHitBox = HitBoxes[strikeVFXIndex];
            // activeHitBox.gameObject.SetActive(true);
            if (++strikeVFXIndex >= StrikeVFXs.Length)
            {
                strikeVFXIndex = 0;
            }
        }
    }
    /*
    public void WeaponHit(IDamageable hit)
    {
        hit.TakeDamage(damage);
        ui.ShowHitMarker();
        //Instantiate(enemyHitParticle, hit, transform.rotation);
    }
    */

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
