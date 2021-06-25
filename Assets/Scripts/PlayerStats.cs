using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    int playerHealth = 100;
    [SerializeField] int playerHealthMax = 100;

    //Return player health
    public int GetHealth()
    {
        return playerHealth;
    }

    //Return bool if player is dead
    public bool IsDead()
    {
        if (playerHealth <= 0)
            return true;
        else
            return false;
    }

    //Damages player by int damage, then checks if they died from that damage.
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }

    //Damages player by int damage, then checks if they died from that damage. Other parameters are not used by the player.
    public void TakeDamage(int damage, Vector3 cutDirection, Vector3 cutPoint)
    {
        playerHealth -= damage;
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }

    //Damages player by int damage, then checks if they died from that damage. Force is not used, as it is calculated as a velocity by the explosion.
    public void TakeExplosiveDamage(int damage, float force)
    {
        playerHealth -= damage;
        if (IsDead())
            Debug.Log("PLAYER DIED");
    }
}
