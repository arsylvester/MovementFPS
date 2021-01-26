using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCourse : MonoBehaviour
{
    [SerializeField] int enemiesKiledMax;
    private DummyEnemy[] targets;
    private PlayerController player;
    private bool isTiming;
    private float currentTime;
    private static float bestTime;
    private static float bestChalTime;
    private int enemiesKilled;
    private UI ui;

    // Start is called before the first frame update
    void Start()
    {
        ui = FindObjectOfType<UI>();
        targets = FindObjectsOfType<DummyEnemy>();
        ui.SetTiming(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming)
        {
            currentTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        if (isTiming == false)
        {
            currentTime = 0;
            isTiming = true;
            ResetEnemyCount();
            ui.SetTiming(true);
            foreach(DummyEnemy target in targets)
            {
                target.gameObject.SetActive(true);
            }
        }
    }

    public void StopTimer()
    {
        isTiming = false;
        if (currentTime < bestTime || bestTime == 0)
        {
            bestTime = currentTime;
        }
        if(enemiesKilled >= enemiesKiledMax && (currentTime < bestChalTime || bestChalTime == 0))
        {
            bestChalTime = currentTime;
        }
        ui.SetTiming(false);
    }

    public void CountEnemies()
    {
        enemiesKilled++;
    }

    public void ResetEnemyCount()
    {
        enemiesKilled = 0;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public int GetCurrentEnemyCount()
    {
        return enemiesKilled;
    }

    public int GetEnemyMax()
    {
        return enemiesKiledMax;
    }
    public float GetBestTime()
    {
        return bestTime;
    }

    public float GetBestChallengeTime()
    {
        return bestChalTime;
    }
}
