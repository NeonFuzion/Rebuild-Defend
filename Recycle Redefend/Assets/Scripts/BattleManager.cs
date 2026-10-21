using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] float restDuration, waveDuration;
    [SerializeField] AudioClip waveStartAudioClip, waveEndAudioClip, countdownAudioClip;
    [SerializeField] Health health;
    [SerializeField] UIButton fastForwardButton;
    [SerializeField] TextMeshProUGUI restText, stateText, waveText;

    float baseTime;
    int timeRemaining, enemiesRemaining, currentWave;
    bool isResting;

    event EventHandler onWaveStarted, onWaveEnded, onTurretHoverExit;
    event EventHandler<int> onTurretHoverEnter;
    event EventHandler<AddItemArgs> onShipmentCollected;
    public class AddItemArgs : EventArgs
    {
        public ItemSO Item;
        public int Amount;
    }
    event EventHandler<TurretRecycleArgs> onTurretRecycled;
    public class TurretRecycleArgs : EventArgs
    {
        public TurretRecipeSO TurretRecipe;
        public Vector2 Position;
    }

    public bool IsInCombat { get; private set; }
    public int CurrentWave => currentWave;

    void Awake()
    {
        if (Instance) return;
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWave = 0;
        isResting = false;
        fastForwardButton.OnClick.AddListener(() =>
        {
            if (IsInCombat) return;
            isResting = false;
        });

        EndWave();
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
        /*else
        {
            int newTime = Mathf.RoundToInt(baseTime - Time.time);

            if (newTime == timeRemaining) return;
            timeRemaining = newTime;
            restText.SetText("Time left before next wave: " + timeRemaining);

            if (timeRemaining <= 0)
                SoundEffectsManager.Instance.PlaySoundEffect(waveStartAudioClip, transform.position, 1);
            else if (timeRemaining <= 5)
                SoundEffectsManager.Instance.PlaySoundEffect(countdownAudioClip, transform.position, 1);
            
        }*/
        
    }

    IEnumerator WaveCoroutine()
    {
        IsInCombat = true;
        waveText.SetText("Wave " + ++currentWave);
        SoundEffectsManager.Instance.PlaySoundEffect(waveStartAudioClip, transform.position, 1);
        yield return new WaitWhile(() => SpawnerManager.Instance.AreEnemiesActive);
        EndWave();
    }

    IEnumerator RestCoroutine()
    {
        IsInCombat = false;
        isResting = true;
        SoundEffectsManager.Instance.PlaySoundEffect(waveEndAudioClip, transform.position, 1);
        baseTime = Time.time + restDuration;
        yield return new WaitWhile(() => isResting);
        StartWave();
    }

    public void StartWave()
    {
        onWaveStarted?.Invoke(this, EventArgs.Empty);
        StartCoroutine(WaveCoroutine());
    }

    public void EndWave()
    {
        onWaveEnded?.Invoke(this, EventArgs.Empty);
        StartCoroutine(RestCoroutine());
    }

    public void DealDamage(int damage)
    {
        bool isDead = health.TakeDamage(damage);

        if (!isDead) return;
        PauseTime();
        UIManager.Instance.EndGame();
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    public void UnpauseTime()
    {
        Time.timeScale = 1;
    }

    public void AddOnWaveStartedListener(EventHandler eventHandler)
    {
        onWaveStarted += eventHandler;
    }

    public void AddOnWaveEndedListener(EventHandler eventHandler)
    {
        onWaveEnded += eventHandler;
    }

    public void AddOnShipmentCollectedListener(EventHandler<AddItemArgs> eventHandler)
    {
        onShipmentCollected += eventHandler;
    }

    public void AddOnTurretRecycledListener(EventHandler<TurretRecycleArgs> eventHandler)
    {
        onTurretRecycled += eventHandler;
    }

    public void AddOnTurretHoverEnterListener(EventHandler<int> eventHandler)
    {
        onTurretHoverEnter += eventHandler;
    }

    public void AddOnTurretHoverExitListener(EventHandler eventHandler)
    {
        onTurretHoverExit += eventHandler;
    }

    public void InvokeOnShipmentCollectedEvent(ItemSO item, int amount)
    {
        onShipmentCollected?.Invoke(this, new AddItemArgs() { Item = item, Amount = amount });
    }

    public void InvokeOnTurretRecycledEvent(TurretRecipeSO turretRecipe, Vector2 position)
    {
        onTurretRecycled?.Invoke(this, new ()
        {
            TurretRecipe = turretRecipe,
            Position = position
        });
    }

    public void InvokeOnTurretHoverEnterEvent(int eventArgs)
    {
        onTurretHoverEnter?.Invoke(this, eventArgs);
    }

    public void InvokeOnTurretHoverExitEvent()
    {
        onTurretHoverExit?.Invoke(this, EventArgs.Empty);
    }
}
