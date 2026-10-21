using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    [SerializeField] bool isActive = true;
    [SerializeField] Vector2 offset = new Vector2(0, 2);
    [SerializeField] AudioClip clickSuccessAudioClip, clickFailAudioClip;
    [SerializeField] GameObject prefabSignifier;
    [SerializeField] UnityEvent onClick, onEnter, onExit;

    GameObject signifier;

    bool isClickable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isClickable) Initialize();
        BattleManager.Instance.AddOnWaveStartedListener((object obj, EventArgs args) => 
        {
            if (!this) return;
            Hide();
        });
        BattleManager.Instance.AddOnWaveEndedListener((object obj, EventArgs args) => 
        {
            if (!this) return;
            Show();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (isClickable)
        {
            SoundEffectsManager.Instance.PlaySoundEffect(clickSuccessAudioClip, transform.position, 1);
            onClick?.Invoke();
        }
        else
        {
            SoundEffectsManager.Instance.PlaySoundEffect(clickFailAudioClip, transform.position, 1);
        }
    }

    void OnMouseEnter()
    {
        onEnter?.Invoke();
    }

    void OnMouseExit()
    {
        onExit?.Invoke();
    }

    void Initialize()
    {
        signifier = Instantiate(prefabSignifier, UIManager.Instance.WorldCanvas);
        signifier.transform.position = (Vector2)transform.position + offset;
    }

    public void Show()
    {
        if (!isActive) return;
        if (!signifier) Initialize();
        isClickable = true; 
        signifier.SetActive(true);
    }

    public void Hide()
    {
        if (!signifier) return;
        isClickable = false;
        signifier.SetActive(false);
    }

    public void Activate()
    {
        isActive = true;
    }

    public void DestroySignifier()
    {
        Destroy(signifier);
    }
}
