using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base abstract weapon script all weapons derive from.
public class Weapon : MonoBehaviour
{
    [SerializeField] protected float coolDown;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float weaponBobHeight;
    [SerializeField] protected float weaponBobSpeed;
    [SerializeField] protected GameObject WeaponModel;
    protected float currentCoolDown;
    protected Transform playerCamera;
    protected UI ui;

    protected virtual void Start()
    {
        playerCamera = Camera.main.transform;
        ui = FindObjectOfType<UI>();
    }

    //Left click, typically the fire/attack.
    public virtual void UseWeapon()
    {

    }

    //Right click, zoom or other special effect.
    public virtual void UseAltFireWeapon()
    {

    }

    //When right click is released, for zoom functionalities nned to know when to unzoom.
    public virtual void AltFireWeaponRelease()
    {

    }

    //Get the model that the player is 'holding'.
    public GameObject GetWeaponModel()
    {
        return WeaponModel;
    }

    // The height the weapon should bob at when the player moves.
    public float GetWeaponBobHeight()
    {
        return weaponBobHeight;
    }

    //The speed the weapon will bob at when the player moves.
    public float GetWeaponBobSpeed()
    {
        return weaponBobSpeed;
    }
}
