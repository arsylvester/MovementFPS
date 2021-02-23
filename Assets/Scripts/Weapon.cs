using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float coolDown;
    [SerializeField] protected int damage = 1;
    protected float currentCoolDown;
    protected Transform playerCamera;
    protected UI ui;

    protected virtual void Start()
    {
        playerCamera = Camera.main.transform;
        ui = FindObjectOfType<UI>();
        print("playerCamera set");
    }

    public virtual void UseWeapon()
    {

    }

    public virtual void UseAltFireWeapon()
    {

    }

    public virtual void AltFireWeaponRelease()
    {

    }
}
