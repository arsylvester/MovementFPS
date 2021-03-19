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
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("Touching " + other.transform.GetComponent<IDamageable>());
        if (other.transform.GetComponent<IDamageable>() != null)
        {
            weapon.WeaponHit(other.transform.GetComponent<IDamageable>());
            //print("Hit enemy");
        }
        else
        {
         
        }
    }
}
