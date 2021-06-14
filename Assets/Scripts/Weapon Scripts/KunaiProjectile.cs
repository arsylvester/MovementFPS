using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    [SerializeField] float renderDelay = .5f;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject enemyHitParticle;
    private float throwSpeed;
    private bool shouldMove = true;
    private int damageToDeal;
    private Vector3 lastPoint;
    private Vector3 lastPointDirection;
    private RaycastHit hit;
    private Renderer[] rends;
    private UI ui;


    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        ToggleAllRenderers(false);
        StartCoroutine(DelayRender());
    }

    void Update()
    {
        if (shouldMove)
        {
            //Move projectile forward
            transform.Translate(transform.forward * throwSpeed * Time.deltaTime, Space.World);

            //Raycast from the current frame's position to last frame's position. This should make a more precise colision detection.
            lastPointDirection = transform.position - lastPoint;
            //Debug.DrawRay(transform.position, lastPointDirection, Color.cyan, Vector3.Distance(transform.position, lastPoint));
            if(Physics.Raycast(lastPoint, lastPointDirection, out hit, Vector3.Distance(transform.position, lastPoint)))
            {
                if (hit.transform.tag != "Player" && !hit.transform.GetComponent<Collider>().isTrigger)
                {
                    shouldMove = false;
                    if (hit.transform.GetComponentInParent<IDamageable>() != null)
                    {
                        hit.transform.GetComponentInParent<IDamageable>().TakeDamage(damageToDeal);
                        ui.ShowHitMarker();
                        Instantiate(enemyHitParticle, transform.position, transform.rotation);
                        AkSoundEngine.PostEvent("KunaiHitEnemy", hit.transform.gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Instantiate(hitParticle, transform.position, transform.rotation);
                        AkSoundEngine.PostEvent("KunaiHit", gameObject);
                    }
                }
            }
            lastPoint = transform.position;
        }
    }

    //Used by the weapon that spawns this projectile to adjust values.
    public void SetParameters(float speed, int damage, UI uiToSet)
    {
        throwSpeed = speed;
        damageToDeal = damage;
        lastPoint = transform.position;
        ui = uiToSet;
    }

    private void ToggleAllRenderers(bool enable)
    {
        foreach (Renderer rend in rends)
        {
            rend.enabled = enable;
        }
    }

    //Used to make the projectile invisible breifly when thrown. Looks better this way
    private IEnumerator DelayRender()
    {
        yield return new WaitForSeconds(renderDelay);
        ToggleAllRenderers(true);
    }

}
