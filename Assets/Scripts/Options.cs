using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public void ChangeSensitivity(float value)
    {
        PlayerController.mouseSensitivity = value;
    }

    public void ChangeVolume(float value)
    {

    }

    public void EnableHeadTilt(bool value)
    {
        PlayerController.tiltHead = value;
    }
}
