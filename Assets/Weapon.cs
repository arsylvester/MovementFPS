using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    public virtual void UseWeapon()
    {

    }
}
