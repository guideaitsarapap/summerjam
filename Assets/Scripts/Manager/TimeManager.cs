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
    [SerializeField] public ShowImageUI showImageUI;

    [Header("HitStop Settings")]
    [SerializeField] private float hitStopDuration = 0.5f;
    [SerializeField] private float slowMoDuration = 1f;
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
    public void DoHitStop(bool useSlowMo, float customDuration = 0f)
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(HitStopRoutine(useSlowMo, customDuration));
    }

    private IEnumerator HitStopRoutine(bool useSlowMo, float customHitStopDuration = 0f)
    {
        Time.timeScale = useSlowMo ? timeScaleIntensity : 0f;
        float duration = useSlowMo ? slowMoDuration : customHitStopDuration < 0.4f ? customHitStopDuration : hitStopDuration;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        hitStopCoroutine = null;
    }

    // --- 2. ระบบนับถอยหลัง 3-2-1 (Pre-Round) ---
    public void StartPreRoundCountdown(int seconds, Action onFinished)
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        isTimerRunning = false; // หยุดเวลา 60 วิไว้ก่อน (ถ้ามี)

        UIManager.Instance.SetEnableUIComponent(UIType.Lobby, false);
        UIManager.Instance.SetEnableUIComponent(UIType.Setting, false);
        UIManager.Instance.SetEnableUIComponent(UIType.CountDown, true);
        UIManager.Instance.SetEnableUIComponent(UIType.Game, true);

        countdownCoroutine = StartCoroutine(PreRoundRoutine(seconds, onFinished));
    }

    private IEnumerator PreRoundRoutine(int seconds, Action onFinished)
    {
        if (showImageUI != null)
        {
            showImageUI.SetEnable(true);
        }

        timerText.gameObject.SetActive(true);
        int currentCount = seconds;

        while (currentCount > 0)
        {
            if (showImageUI != null)
            {
                showImageUI.UpdateCountdownDisplay(currentCount);
            }

            yield return new WaitForSecondsRealtime(1f);
            currentCount--;
        }
        
        if (showImageUI != null)
        {
            showImageUI.UpdateCountdownDisplay(0); // เลข 0 แทนรูป GO!
        }

        //timerText.text = "FIGHT!";   หรือไม่ก็ทำ UI มาแทน Text
        onFinished?.Invoke(); // บอก GameFlow ว่าเริ่มเล่นได้

        yield return new WaitForSecondsRealtime(0.5f);

        if (showImageUI != null)
        {
            showImageUI.SetEnable(false);
        }
        
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