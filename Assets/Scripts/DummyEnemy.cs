using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 5;
    public GameObject vfx;
    int health;
    Vector3 cutDirection;
    Vector3 cutPoint;
    bool canCutChild;

    //Make sure start with full health
    void Start()
    {
        health = maxHealth;
    }

    //For none cutable enemies have the health reset when reenabled
    private void OnEnable()
    {
        health = maxHealth;
    }

    //When the enemy dies play a VFX and either cut/crumble depending on the enemy and attack, or just deactivate.
    private void Death()
    {
        FindObjectOfType<TimedCourse>().CountEnemies();
        Destroy(Instantiate(vfx, transform.position, transform.rotation), 2); //have vfx self destroy later
        MeshDestroy crumbleEffect = GetComponent<MeshDestroy>();
        if(crumbleEffect)
        {
            crumbleEffect.DestroyMesh(cutDirection, cutPoint, canCutChild);
        }
        else
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    //This take damage takes into account cut and point hit, so if the damage is to kill the enemy they will cut/crumble correctly.
    public void TakeDamage(int damage, Vector3 cut, Vector3 point)
    {
        health -= damage;

        if (IsDead())
        {
            cutDirection = cut;
            cutPoint = point;
            canCutChild = true;
            Death();
        }
    }

    //Take damage but set cut and point to default (Cuts straight in half vertically)
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (IsDead())
        {
            cutDirection = Vector3.left;
            cutPoint = Vector3.zero;
            canCutChild = false;
            Death();
        }
    }

    //Explosive damage to set a crumble effect with force if it would kill the enemy.
    public void TakeExplosiveDamage(int damage, float force)
    {
        //Only change crumble force if this would kill the enemy.
        if (health - damage <= 0)
        {
            MeshDestroy crumbleEffect = GetComponent<MeshDestroy>();
            crumbleEffect.ExplodeForce = force;
            crumbleEffect.CutCascades = 4;
        }
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
