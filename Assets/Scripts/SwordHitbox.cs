using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private MeleeWeapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInParent<MeleeWeapon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<IDamageable>() != null)
        {
           // weapon.WeaponHit(other.transform.GetComponent<IDamageable>());
            print("Hit enemy");
        }
        else
        {
         
        }
    }
}
