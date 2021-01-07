﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
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
