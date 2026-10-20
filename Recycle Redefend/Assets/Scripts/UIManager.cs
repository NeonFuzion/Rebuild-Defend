using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Transform worldCanvas;

    int appearAnimHash = Animator.StringToHash("CraftingAppear"),
        disappearAnimHash = Animator.StringToHash("CraftingDisappear");

    Animator animator;

    public Transform WorldCanvas => worldCanvas;

    void Awake()
    {
        if (Instance) return;
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        BattleManager.Instance.AddOnWaveEndedListener((object obj, EventArgs args) => OnWaveEnded());
        BattleManager.Instance.AddOnWaveStartedListener((object obj, EventArgs args) => OnWaveStarted());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnWaveStarted()
    {
        animator.CrossFade(disappearAnimHash, 0, 0);
    }

    void OnWaveEnded()
    {
        animator.CrossFade(appearAnimHash, 0, 0);
    }
}
