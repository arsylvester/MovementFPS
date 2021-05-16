using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] GameObject objectAtEnd;
    [SerializeField] float zoomAmount = 55;
    [SerializeField] float zoomSpeed = .05f;
    //[SerializeField] protected bool hitScan;

    protected RaycastHit hit;
    protected bool isHit;

    float oldZoom;
    Camera cam;

    protected override void Start()
    {
        base.Start();
        cam = playerCamera.GetComponent<Camera>();
    }

    public override void UseWeapon()
    {

        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 1000))
        {
            print("Projectile hit : " + hit.transform);
            isHit = true;
            //Instantiate(objectAtEnd, hit.point, playerCamera.rotation);
        }
        else
        {
            isHit = false;
        }
    }

    public override void UseAltFireWeapon()
    {
        oldZoom = PlayerController.fovValue;
        StartCoroutine(Zoom(oldZoom, zoomAmount));
    }

    public override void AltFireWeaponRelease()
    {
        StartCoroutine(Zoom(zoomAmount, oldZoom));
    }

    IEnumerator Zoom(float from, float to)
    {
        float t = 0;
        while(t < 1)
        {
            cam.fieldOfView = Mathf.Lerp(from, to, t);
            t += zoomSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
