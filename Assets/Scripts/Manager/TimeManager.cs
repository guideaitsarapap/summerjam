using UnityEngine;
using System.Collections;
using TMPro;
using System;

namespace Scripts.UI
{
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("HitStop Settings")]
    [SerializeField] private float hitStopDuration = 1f;
    [SerializeField] [Range(0, 1)] private float timeScaleIntensity = 0f;

    [Header("Round Settings")]
    [SerializeField] private int maxRoundTime = 60;
    private float remainingRoundTime;
    private bool isTimerRunning = false;

    private Coroutine hitStopCoroutine;
    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // อัปเดตเวลาระหว่างเล่น (60 วินาที)
        if (isTimerRunning && remainingRoundTime > 0)
        {
            remainingRoundTime -= Time.deltaTime;
            UpdateTimerUI(remainingRoundTime);

            if (remainingRoundTime <= 0)
            {
                OnTimeOut();
            }
        }
    }

    // --- 1. ระบบ HitStop ---
    public void DoHitStop()
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(HitStopRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        Time.timeScale = timeScaleIntensity;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
        hitStopCoroutine = null;
    }

    // --- 2. ระบบนับถอยหลัง 3-2-1 (Pre-Round) ---
    public void StartPreRoundCountdown(int seconds, Action onFinished)
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        isTimerRunning = false; // หยุดเวลา 60 วิไว้ก่อน (ถ้ามี)
        countdownCoroutine = StartCoroutine(PreRoundRoutine(seconds, onFinished));
    }

    private IEnumerator PreRoundRoutine(int seconds, Action onFinished)
    {
        timerText.gameObject.SetActive(true);
        int currentCount = seconds;

        while (currentCount > 0)
        {
            //อยากทำ UI เลขใหญ่ๆกลางจอทำตรงนี้
            timerText.text = currentCount.ToString();
            yield return new WaitForSecondsRealtime(1f);
            currentCount--;
        }

        //timerText.text = "FIGHT!";   หรือไม่ก็ทำ UI มาแทน Text
        onFinished?.Invoke(); // บอก GameFlow ว่าเริ่มเล่นได้

        yield return new WaitForSecondsRealtime(0.5f);
        
        // เริ่มนับเวลาถอยหลัง 60 วินาทีของรอบ
        StartRoundTimer();
    }

    // --- 3. ระบบเวลารอบ (Round Timer 60s) ---
    private void StartRoundTimer()
    {
        remainingRoundTime = maxRoundTime;
        isTimerRunning = true;
    }

    private void UpdateTimerUI(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        
        int seconds = Mathf.FloorToInt(timeToDisplay);
        
        timerText.text = seconds.ToString();
    }

    private void OnTimeOut()
    {
        isTimerRunning = false;
        timerText.text = "TIME UP!";
        Debug.Log("Round Time Ended!");
        
        // แจ้ง GameFlowManager ว่าหมดเวลา (Draw หรือหาผู้ชนะจากเลือด)
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.HandleTimeOut();
        }
    }

    public void StopTimer() // ใช้เรียกตอนมีคนตายก่อนหมดเวลา
    {
        isTimerRunning = false;
    }
}