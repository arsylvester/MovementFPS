using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damageToDeal;
    private Vector3 lastPoint;
    private Vector3 lastPointDirection;
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        lastPoint = transform.position;
    }

    //Checks both a raycast and an ontrigger. The trigger is mainly for the player. This may potential cause double damage to things and the trigger could be bad detection on the player. Look to improve.

    // Update is called once per frame
    void Update()
    {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            lastPointDirection = transform.position - lastPoint;
            if (Physics.Raycast(lastPoint, lastPointDirection, out hit, Vector3.Distance(transform.position, lastPoint)))
            {
                if (!hit.transform.GetComponent<Collider>().isTrigger)
                {
                    if (hit.transform.GetComponentInParent<IDamageable>() != null)
                    {
                        hit.transform.GetComponentInParent<IDamageable>().TakeDamage(damageToDeal);
                        AkSoundEngine.PostEvent("KunaiHitEnemy", hit.transform.gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        AkSoundEngine.PostEvent("KunaiHit", gameObject);
                    }

                    Destroy(gameObject);
                }
            }
            lastPoint = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.GetComponent<Collider>().isTrigger)
        {
            if (other.transform.GetComponentInParent<IDamageable>() != null)
            {
                other.transform.GetComponentInParent<IDamageable>().TakeDamage(damageToDeal);
                AkSoundEngine.PostEvent("KunaiHitEnemy", other.transform.gameObject);
                Destroy(gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent("KunaiHit", gameObject);
            }

            Destroy(gameObject);
        }
    }

    public void SetParameters(float speed, int damage, UI uiToSet)
    {
        this.speed = speed;
        damageToDeal = damage;
        lastPoint = transform.position;
    }
}
