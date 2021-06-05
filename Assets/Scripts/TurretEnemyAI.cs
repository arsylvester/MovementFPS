using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemyAI : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] float fireRate = .5f;
    [SerializeField] Transform projectileSpawnLocation;
    [SerializeField] float detectionRange = 10;
    public GameObject vfx;
    int health;
    Vector3 cutDirection;
    Vector3 cutPoint;
    bool canCutChild;
    bool isFiring = true;
    float fireCooldown;
    Transform player;


    void Start()
    {
        health = maxHealth;
        player = FindObjectOfType<PlayerController>().transform;
    }

    private void OnEnable()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            if (fireCooldown + fireRate < Time.time)
            {
                Instantiate(enemyProjectile, projectileSpawnLocation.position, projectileSpawnLocation.rotation);
                fireCooldown = Time.time;
            }
            transform.LookAt(player);
        }
    }

    private void Death()
    {
        FindObjectOfType<TimedCourse>().CountEnemies();
        Destroy(Instantiate(vfx, transform.position, transform.rotation), 2); //have vfx self destroy later
        MeshDestroy crumbleEffect = GetComponent<MeshDestroy>();
        if (crumbleEffect)
        {
            crumbleEffect.DestroyMesh(cutDirection, cutPoint, canCutChild);
        }
        else
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage, Vector3 cut, Vector3 point)
    {
        print("Was dealt " + damage + " damage");
        health -= damage;
        cutDirection = cut;
        cutPoint = point;
        canCutChild = true;
        if (IsDead())
        {
            Death();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        cutDirection = Vector3.left;
        cutPoint = Vector3.zero;
        canCutChild = false;
        if (IsDead())
        {
            Death();
        }
    }

    public void TakeExplosiveDamage(int damage, float force)
    {
        //MeshDestroy crumbleEffect = GetComponent<MeshDestroy>();
        //crumbleEffect.ExplodeForce = force;
        //crumbleEffect.CutCascades = 4;
        TakeDamage(damage);
    }


    public bool IsDead()
    {
        return health <= 0;
    }

    public int GetHealth()
    {
        return health;
    }
}
