using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float coolDown;
    [SerializeField] protected int damage = 1;
    protected float currentCoolDown;
    protected Transform playerCamera;

    protected virtual void Start()
    {
        playerCamera = Camera.main.transform;
        print("playerCamera set");
    }

    public virtual void UseWeapon()
    {

    }
}
