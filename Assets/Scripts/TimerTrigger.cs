using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [SerializeField] bool startsTimer = true;
    TimedCourse timedCourse;

    private void Start()
    {
        timedCourse = FindObjectOfType<TimedCourse>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            if(startsTimer)
            {
                timedCourse.StartTimer();
            }
            else
            {
                timedCourse.StopTimer();
            }
        }
    }
}
