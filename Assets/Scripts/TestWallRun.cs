using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWallRun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool wallRight = false, wallLeft = false;
        RaycastHit hitRight, hitLeft;
        Transform wallHit = null;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitRight, 10))
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 10))
            {
                if (hitLeft.distance < hitRight.distance)
                {
                    wallRight = false;
                    wallHit = hitLeft.transform;
                }
                else
                {
                    wallHit = hitRight.transform;
                    wallRight = true;
                }
            }
            else
            {
                wallRight = true;
                wallHit = hitRight.transform;
            }
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 10))
        {
            wallRight = false;
            wallHit = hitLeft.transform;
        }

        if(wallRight)
        {
            Debug.DrawLine(transform.position, hitRight.point, Color.green);
            Debug.DrawRay(transform.position, new Vector3(hitRight.normal.z, 0, -hitRight.normal.x), Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, hitLeft.point, Color.green);
            Debug.DrawRay(transform.position, new Vector3(-hitLeft.normal.z, 0, hitLeft.normal.x), Color.red);
        }
    }
}
