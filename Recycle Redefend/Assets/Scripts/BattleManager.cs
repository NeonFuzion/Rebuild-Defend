using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] float restDuration, waveDuration;
    [SerializeField] AudioClip waveStartAudioClip, waveEndAudioClip, countdownAudioClip;
    [SerializeField] GameObject shipment;
    [SerializeField] Health health;
    [SerializeField] TextMeshProUGUI stateText;

    float baseTime;
    int timeRemaining, enemiesRemaining;

    event EventHandler onWaveStarted, onWaveEnded;

    public bool IsInCombat { get; private set; }

    void Awake()
    {
        if (Instance) return;
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeOnWaveEndedEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInCombat)
        {
            int currentEnemiesRemaining = SpawnerManager.Instance.ActiveEnemyCount;

            if (currentEnemiesRemaining == enemiesRemaining) return;
            enemiesRemaining = currentEnemiesRemaining;
            stateText.SetText("Enemies left in current wave: " + enemiesRemaining);
        }
        else
        {
            int newTime = Mathf.RoundToInt(baseTime - Time.time);

            if (newTime == timeRemaining) return;
            timeRemaining = newTime;
            stateText.SetText("Time left before next wave: " + timeRemaining);

            if (timeRemaining <= 0)
                SoundEffectsManager.Instance.PlaySoundEffect(waveStartAudioClip, transform.position, 1);
            else if (timeRemaining <= 5)
                SoundEffectsManager.Instance.PlaySoundEffect(countdownAudioClip, transform.position, 1);

        }
        
    }

    IEnumerator WaveCoroutine()
    {
        IsInCombat = true;
        yield return new WaitWhile(() => SpawnerManager.Instance.AreEnemiesActive);
        InvokeOnWaveEndedEvent();
    }

    IEnumerator RestCoroutine()
    {
        IsInCombat = false;
        SoundEffectsManager.Instance.PlaySoundEffect(waveEndAudioClip, transform.position, 1);
        baseTime = Time.time + restDuration;
        yield return new WaitForSeconds(restDuration);
        InvokeOnWaveStartedEvent();
    }

    public void DealDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    public void AddOnWaveStartedListener(EventHandler eventHandler)
    {
        onWaveStarted += eventHandler;
    }

    public void AddOnWaveEndedListener(EventHandler eventHandler)
    {
        onWaveEnded += eventHandler;
    }

    public void InvokeOnWaveStartedEvent()
    {
        onWaveStarted?.Invoke(this, EventArgs.Empty);
        StartCoroutine(WaveCoroutine());
    }

    public void InvokeOnWaveEndedEvent()
    {
        onWaveEnded?.Invoke(this, EventArgs.Empty);
        StartCoroutine(RestCoroutine());
    }
}
