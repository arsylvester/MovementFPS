using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Text speedText;
    [SerializeField] Slider dashSlider;
    [SerializeField] Image dashFill;
    [SerializeField] Color unreadyDashColor;
    [SerializeField] Color readyDashColor;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.visible = false;
        dashFill.color = readyDashColor;
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = player.GetCurrentSpeed();
        if (currentSpeed < .01f)
            currentSpeed = 0;
        speedText.text = "Speed:\n" + currentSpeed;

        dashSlider.value = Mathf.Clamp(player.GetDashPercent(), 0, 1);
        if (dashSlider.value < 1)
        {
            dashFill.color = unreadyDashColor;
        }
        else
        {
            dashFill.color = readyDashColor;
        }
    }
}
