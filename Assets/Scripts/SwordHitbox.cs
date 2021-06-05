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

    //If collides with damageable object thats not a player, call the damage funtion for the weapon.
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<IDamageable>() != null && other.GetComponent<PlayerController>() == null)
        {
            weapon.WeaponHit(other.transform.GetComponentInParent<IDamageable>());
        }
        else
        {
         
        }
    }
}
