using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] GameObject vfx;
    int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Death()
    {
        FindObjectOfType<UI>().CountEnemies();
        Destroy(Instantiate(vfx, transform.position, transform.rotation), 2); //have vfx self destroy later
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(IsDead())
        {
            Death();
        }
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
