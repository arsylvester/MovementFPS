using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Text speedText;
    [SerializeField] Text EnemiesText;
    [SerializeField] Text TimeText;
    [SerializeField] Slider dashSlider;
    [SerializeField] Image dashFill;
    [SerializeField] Color unreadyDashColor;
    [SerializeField] Color readyDashColor;
    [SerializeField] Image hitMarker;
    [SerializeField] float hitMarkerDelay = .1f;
    [SerializeField] bool timingIsDisplayed;
    [SerializeField] float enemiesKiledMax;
    private PlayerController player;
    private bool isTiming;
    private float currentTime;
    private float bestTime;
    private float enemiesKilled;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        dashFill.color = readyDashColor;
        hitMarker.gameObject.SetActive(false);

        if(timingIsDisplayed)
        {
            EnemiesText.gameObject.SetActive(true);
            TimeText.gameObject.SetActive(true);
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
            currentTime += Time.deltaTime;
            TimeText.text = "Time: " + currentTime.ToString("F2");
            EnemiesText.text = "Targets Destroyed: " + enemiesKilled + "/" + enemiesKiledMax;
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

    public void StartTimer()
    {
        if (isTiming == false)
        {
            currentTime = 0;
            isTiming = true;
            ResetEnemyCount();
        }
    }

    public void StopTimer()
    {
        isTiming = false;
        if(currentTime > bestTime)
        {
            bestTime = currentTime;
        }
    }

    public void CountEnemies()
    {
        enemiesKilled++;
    }

    public void ResetEnemyCount()
    {
        enemiesKilled = 0;
    }
}
