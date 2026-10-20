using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] int minItems = 10, maxItems = 15;
    [SerializeField] Transform worldCanvas;
    [SerializeField] ItemSO[] items;
    [SerializeField] Transform optionParent;

    int appearAnimHash = Animator.StringToHash("CraftingAppear"),
        disappearAnimHash = Animator.StringToHash("CraftingDisappear"),
        shipmentOpenAnimHash = Animator.StringToHash("ShipmentOpen"),
        shipmentCloseAnimHash = Animator.StringToHash("ShipmentClose");

    Animator animator;
    ShipmentOption[] shipmentOptions;

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
        shipmentOptions = optionParent.GetComponentsInChildren<ShipmentOption>();

        BattleManager.Instance.AddOnWaveEndedListener((object obj, System.EventArgs args) => OnWaveEnded());
        BattleManager.Instance.AddOnWaveStartedListener((object obj, System.EventArgs args) => OnWaveStarted());
        BattleManager.Instance.AddOnShipmentCollectedListener((object obj, BattleManager.AddItemArgs args) => CloseShipment());
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

    void OnAnimationOpenShipment()
    {
        List<ItemSO> itemPool = items.ToList();
        foreach (ShipmentOption option in shipmentOptions)
        {
            int index = Random.Range(0, itemPool.Count);
            ItemSO item = itemPool[index];
            itemPool.RemoveAt(index);
            int amount = Random.Range(minItems, maxItems);
            option.Initialize(item, amount);
        }
    }

    public void OpenShipment()
    {
        animator.CrossFade(shipmentOpenAnimHash, 0, 0);
    }

    public void CloseShipment()
    {
        animator.CrossFade(shipmentCloseAnimHash, 0, 0);
    }
}
