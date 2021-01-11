using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    [SerializeField] float renderDelay = .5f;
    private float throwSpeed;
    private bool shouldMove = true;
    private int damageToDeal;
    private Vector3 lastPoint;
    private Vector3 lastPointDirection;
    private RaycastHit hit;
    private Renderer[] rends;
    private UI ui;


    // Start is called before the first frame update
    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        ToggleAllRenderers(false);
        StartCoroutine(DelayRender());
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            transform.Translate(transform.forward * throwSpeed * Time.deltaTime, Space.World);
            lastPointDirection = transform.position - lastPoint;
            Debug.DrawRay(transform.position, lastPointDirection, Color.cyan, Vector3.Distance(transform.position, lastPoint));
            if(Physics.Raycast(transform.position, lastPointDirection, out hit, Vector3.Distance(transform.position, lastPoint)))
            {
                if (hit.transform.tag != "Player")
                {
                    shouldMove = false;
                    if (hit.transform.GetComponent<IDamageable>() != null)
                    {
                        hit.transform.GetComponent<IDamageable>().TakeDamage(damageToDeal);
                        ui.ShowHitMarker();
                    }
                }
            }
            lastPoint = transform.position;
        }
    }

    public void SetParameters(float speed, int damage, UI uiToSet)
    {
        throwSpeed = speed;
        damageToDeal = damage;
        lastPoint = transform.position;
        ui = uiToSet;
    }

    private void ToggleAllRenderers(bool enable)
    {
        foreach (Renderer rend in rends)
        {
            rend.enabled = enable;
        }
    }

    private IEnumerator DelayRender()
    {
        yield return new WaitForSeconds(renderDelay);
        ToggleAllRenderers(true);
    }

}
