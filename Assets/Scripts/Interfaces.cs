using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    void TakeDamage(int damage, Vector3 cutDirection, Vector3 cutPoint);
    int GetHealth();
    bool IsDead();
}
