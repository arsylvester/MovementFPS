using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    int playerHealth = 100;
    [SerializeField] int playerHealthMax = 100;

    public int GetHealth()
    {
        return playerHealth;
    }

    public bool IsDead()
    {
        if (playerHealth <= 0)
            return true;
        else
            return false;
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }

    public void TakeDamage(int damage, Vector3 cutDirection, Vector3 cutPoint)
    {
        playerHealth -= damage;
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }

    public void TakeExplosiveDamage(int damage, float force)
    {
        playerHealth -= damage;
        Debug.Log("PLAYER Damaged");
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }
}
