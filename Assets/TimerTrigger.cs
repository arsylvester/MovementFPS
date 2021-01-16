using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [SerializeField] bool startsTimer = true;
    UI ui;

    private void Start()
    {
        ui = FindObjectOfType<UI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            if(startsTimer)
            {
                ui.StartTimer();
            }
            else
            {
                ui.StopTimer();
            }
        }
    }
}
