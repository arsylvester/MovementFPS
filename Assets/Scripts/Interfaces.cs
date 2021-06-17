using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For all enemies, the player, and likely destructible objects.
public interface IDamageable
{
    void TakeDamage(int damage);
    void TakeDamage(int damage, Vector3 cutDirection, Vector3 cutPoint);
    void TakeExplosiveDamage(int damage, float force);
    int GetHealth();
    bool IsDead();
}
