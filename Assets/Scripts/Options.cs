﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] Slider mouseSense;
    [SerializeField] Slider volume;
    [SerializeField] Slider fovSlider;
    [SerializeField] Toggle headTilt;

    private void Start()
    {
        mouseSense.value = PlayerController.mouseSensitivity;
        headTilt.isOn = PlayerController.tiltHead;
        fovSlider.value = PlayerController.fovValue;
    }

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

    public void ChangeFOV(float value)
    {
        //PlayerController.fovValue = value;
        PlayerController.SetFOV(value);
    }
}
