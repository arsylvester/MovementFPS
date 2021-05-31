using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    //[SerializeField] protected bool hitScan;
    [SerializeField] float meleeRange = 1;
    [SerializeField] float parryRange = 1;
    [SerializeField] float holsterCoolDown = .5f;
    [SerializeField] ParticleSystem[] StrikeVFXs;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject enemyHitParticle;
    [SerializeField] Renderer[] swordRenderers;
    [SerializeField] Collider DashHitBox;
    [SerializeField] ParticleSystem dashVFX;
    [SerializeField] ParticleSystem parryVFX;
    [SerializeField] float backstabDegree;
    [SerializeField] float parryCooldown = .5f;
    [SerializeField] float slowdownTimeScale = .05f;
    [SerializeField] float slowdownTimeduration = .25f;
    //[SerializeField] Collider[] HitBoxes;

    protected RaycastHit hit;
    protected bool isHit;

    private int strikeVFXIndex;
    private float parryCurrentCooldown;
    //private Collider activeHitBox;

    protected override void Start()
    {
        base.Start();
        //activeHitBox = HitBoxes[0];
    }

    private void Update()
    {
        //Re-enable the sword renderer after a brief time without using the main or alt ability. Also reset the VFX to the first in the list.
        if (!(swordRenderers[0].enabled) && coolDown + holsterCoolDown + currentCoolDown < Time.time && parryCooldown + parryCurrentCooldown + holsterCoolDown < Time.time)
        {
            ToggleAllRenderers(true);
            //activeHitBox.gameObject.SetActive(false);
            strikeVFXIndex = 0;
        }
    }

    public override void UseWeapon()
    {
        //If enough time has passed since last attack
        if (coolDown + currentCoolDown < Time.time && parryCooldown + parryCurrentCooldown < Time.time)
        {
            currentCoolDown = Time.time;
            ToggleAllRenderers(false);
            
            //Raycast to see if hits an object. If the object can be damaged deal damage.
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, meleeRange))
            {
                isHit = true;
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
    
    //Used for special abilities such as the dash attack, so that the proper damage is used.
    public void WeaponHit(IDamageable hit)
    {
        hit.TakeDamage(damage, dashVFX.transform.up, Vector3.zero);
        ui.ShowHitMarker();
        //Instantiate(enemyHitParticle, hit, transform.rotation);
    }

    //On a dash while the sword is equiped, this is called.
    public void DashAttack()
    {
        DashHitBox.enabled = true;
        currentCoolDown = Time.time;
        dashVFX.Play();
        ToggleAllRenderers(false);
    }

    //Disable the dash attack hitbox.
    public void DashEnd()
    {
        DashHitBox.enabled = false;
        ToggleAllRenderers(true);
    }
    
    //For the sword it parries attakcs. 
    //Currently just parries enemy projectiles, sending them directly back the direction they came.
    //Consider: Have projectile move in the direction facing, instead of directly back. or if directly back have it track at a higher speed back to the enemy.
    public override void UseAltFireWeapon()
    {
        if (parryCooldown + parryCurrentCooldown < Time.time)
        {
            parryCurrentCooldown = Time.time;
            ToggleAllRenderers(false);

            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, parryRange))
            {
                isHit = true;
                if (hit.transform.GetComponent<EnemyProjectile>())
                {
                    hit.transform.GetComponent<EnemyProjectile>().reverseDirection();
                    AkSoundEngine.PostEvent("Parry", gameObject);
                    parryVFX.Play();
                    StartCoroutine(SlowDownTime());
                }
                parryCurrentCooldown = 0;
            }
            else
            {
                isHit = false;
            }

            StrikeVFXs[strikeVFXIndex].Play();

            if (++strikeVFXIndex >= StrikeVFXs.Length)
            {
                strikeVFXIndex = 0;
            }
            AkSoundEngine.PostEvent("SwordSwing", gameObject);
        }
    }

    //Not used, but inherited
    public override void AltFireWeaponRelease()
    {
        //StartCoroutine(Zoom(zoomAmount, oldZoom));
    }

    //Show sword or not
    private void ToggleAllRenderers(bool enable)
    {
        foreach (Renderer rend in swordRenderers)
        {
            rend.enabled = enable;
        }
    }

    //Slows down time very briefly. Gives impact to the parry. (Thanks ULTRAKILL :) )
    //Might consider putting this in a more universal script if a similar concept is used elsewhere. But for now this is fine.
    IEnumerator SlowDownTime()
    {
        Time.timeScale = slowdownTimeScale;
        yield return new WaitForSecondsRealtime(slowdownTimeduration);
        Time.timeScale = 1;
    }

}
