using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroPrompt : MonoBehaviour
{
    static bool wasShownBefore = false;
    // Start is called before the first frame update
    void Start()
    {
        if(wasShownBefore)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            wasShownBefore = true;
            gameObject.SetActive(false);
        }
    }
}
