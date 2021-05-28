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
    [SerializeField] Collider DashHitBox;
    [SerializeField] ParticleSystem dashVFX;
    [SerializeField] float backstabDegree;
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
                isHit = true;
                //Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
                if (hit.transform.GetComponentInParent<IDamageable>() != null)
                {
                    Vector3 stabDirection = (hit.transform.position - hit.point);
                    float stabDegree = Mathf.Abs(Mathf.Atan2(stabDirection.z, stabDirection.x) * Mathf.Rad2Deg);
                    float enemyDirection = Mathf.Abs(Mathf.Atan2(hit.transform.forward.z, hit.transform.forward.x) * Mathf.Rad2Deg);
                    //print("Melee hit with hit: " + stabDegree + ", enemy: " + enemyDirection);
                    if (Mathf.Abs(stabDegree - enemyDirection) < backstabDegree)
                    {
                        print("BACKSTAB");
                        hit.transform.GetComponentInParent<IDamageable>().TakeDamage(damage * 2, StrikeVFXs[strikeVFXIndex].transform.up, hit.transform.InverseTransformPoint(hit.point));
                    }
                    else
                    {
                        hit.transform.GetComponentInParent<IDamageable>().TakeDamage(damage, StrikeVFXs[strikeVFXIndex].transform.up, hit.transform.InverseTransformPoint(hit.point));
                    }
                    ui.ShowHitMarker();
                    Instantiate(enemyHitParticle, hit.point, transform.rotation);
                }
                else if(hit.transform.GetComponent<EnemyProjectile>())
                {
                    hit.transform.GetComponent<EnemyProjectile>().reverseDirection();
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
            AkSoundEngine.PostEvent("SwordSwing", gameObject);
        }
    }
    
    public void WeaponHit(IDamageable hit)
    {
        hit.TakeDamage(damage, dashVFX.transform.up, Vector3.zero);
        ui.ShowHitMarker();
        //Instantiate(enemyHitParticle, hit, transform.rotation);
    }

    public void DashAttack()
    {
        DashHitBox.enabled = true;
        currentCoolDown = Time.time;
        dashVFX.Play();
        ToggleAllRenderers(false);
    }

    public void DashEnd()
    {
        DashHitBox.enabled = false;
        ToggleAllRenderers(true);
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
