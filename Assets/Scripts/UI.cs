using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Text speedText;
    [SerializeField] Text EnemiesText;
    [SerializeField] TextMesh bestTimeText;
    [SerializeField] TextMesh bestTimeChalText;
    [SerializeField] Text TimeText;
    [SerializeField] Slider dashSlider;
    [SerializeField] Image dashFill;
    [SerializeField] Color unreadyDashColor;
    [SerializeField] Color readyDashColor;
    [SerializeField] Image hitMarker;
    [SerializeField] float hitMarkerDelay = .1f;
    [SerializeField] bool timingIsDisplayed;
    private PlayerController player;
    private bool isTiming;
    private TimedCourse timedCourse;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        dashFill.color = readyDashColor;
        hitMarker.gameObject.SetActive(false);
        timedCourse = FindObjectOfType<TimedCourse>();

        if(timingIsDisplayed)
        {
            EnemiesText.gameObject.SetActive(true);
            TimeText.gameObject.SetActive(true);
            bestTimeText.gameObject.SetActive(true);
        }
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

        if(isTiming)
        {
            TimeText.text = "Time: " + timedCourse.GetCurrentTime().ToString("F2");
            EnemiesText.text = "Targets Destroyed: " + timedCourse.GetCurrentEnemyCount() + "/" + timedCourse.GetEnemyMax();
        }
    }

    public void ShowHitMarker()
    {
        hitMarker.gameObject.SetActive(true);
        StartCoroutine(HideHitMarker());
    }

    private IEnumerator HideHitMarker()
    {
        yield return new WaitForSeconds(hitMarkerDelay);
        hitMarker.gameObject.SetActive(false);
    }

    public void SetTiming(bool timing)
    {
        isTiming = timing;
        EnemiesText.gameObject.SetActive(timing);
        if(!timing)
        {
            bestTimeText.text = "Best Time: " + timedCourse.GetBestTime().ToString("F2");
            bestTimeChalText.text = "Best Challenge Time: " + timedCourse.GetBestChallengeTime().ToString("F2");
        }
        //TimeText.gameObject.SetActive(timing);
        //bestTimeTest.gameObject.SetActive(timing);
    }
}
