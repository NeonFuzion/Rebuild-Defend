using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] float restDuration, waveDuration;
    [SerializeField] Health health;
    [SerializeField] TextMeshProUGUI stateText;

    string text;
    float baseTime;

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
        stateText.SetText(text + (IsInCombat ?
            SpawnerManager.Instance.ActiveEnemyCount :
            (int)(baseTime - Time.time))
        );
    }

    IEnumerator WaveCoroutine()
    {
        IsInCombat = true;
        text = "Enemies left in current wave: ";
        yield return new WaitWhile(() => SpawnerManager.Instance.AreEnemiesActive);
        InvokeOnWaveEndedEvent();
    }

    IEnumerator RestCoroutine()
    {
        IsInCombat = false;
        baseTime = Time.time + restDuration;
        text = "Time left before next wave: ";
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
