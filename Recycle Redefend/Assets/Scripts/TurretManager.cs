using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TurretManager : MonoBehaviour
{
    [SerializeField] Vector2 placementBounds, durabilityBarOffset = new Vector2(0, -1.25f);
    [SerializeField] DurabilityBar prefabDurabilityBar;
    [SerializeField] ItemSlot prefabItemSlot;
    [SerializeField] AudioClip placeSuccessAudioClip, placeFailAudioClip;
    [SerializeField] Transform itemParent, turretParent;
    [SerializeField] ItemToAmount[] items;
    [SerializeField] UnityEvent onTurretPlaced;

    Vector2 mousePosition;
    TurretRecipeSO currentRecipe;

    TurretButton[] turretButtons;
    Dictionary<ItemSO, AmountToSlot> inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<ItemSO, AmountToSlot>();
        foreach (ItemToAmount pair in items)
        {
            ItemSlot itemSlot = Instantiate(prefabItemSlot, itemParent);
            itemSlot.Initialize(pair.Item.Sprite, pair.Amount);
            inventory.Add(pair.Item, new (pair.Amount, itemSlot));
        }

        turretButtons = new TurretButton[turretParent.childCount];
        for (int i = 0; i < turretButtons.Length; i++)
        {
            TurretButton turretButton = turretParent.GetChild(i).GetComponent<TurretButton>();
            turretButtons[i] = turretButton;
            turretButton.Initialize(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceTurret()
    {
        if (!currentRecipe || Mathf.Abs(mousePosition.x) > placementBounds.x || Mathf.Abs(mousePosition.y) > placementBounds.y)
        {
            SoundEffectsManager.Instance.PlaySoundEffect(placeFailAudioClip, mousePosition, 1);
            return;
        }
        bool isCraftable = true;
        foreach (ItemToAmount pair in currentRecipe.Materials)
        {
            if (pair.Amount <= inventory[pair.Item].Amount) continue;
            isCraftable = false;
        }

        if (!isCraftable)
        {
            SoundEffectsManager.Instance.PlaySoundEffect(placeFailAudioClip, mousePosition, 1);
            return;
        }
        foreach (ItemToAmount pair in currentRecipe.Materials)
        {
            AmountToSlot amountToSlot = inventory[pair.Item];
            amountToSlot.Amount -= pair.Amount;
            amountToSlot.ItemSlot.UpdateAmount(amountToSlot.Amount);
        }

        GameObject turretGameObject = Instantiate(currentRecipe.PrefabTurret, mousePosition, Quaternion.identity);
        DurabilityBar durabilityBar = Instantiate(prefabDurabilityBar, mousePosition + durabilityBarOffset, Quaternion.identity);
        durabilityBar.transform.SetParent(UIManager.Instance.WorldCanvas);
        turretGameObject.GetComponent<Turret>().Initialize(durabilityBar);
        SoundEffectsManager.Instance.PlaySoundEffect(placeSuccessAudioClip, mousePosition, 1);
        onTurretPlaced?.Invoke();
    }

    public void SetTurret(TurretRecipeSO turretRecipe, int index)
    {
        currentRecipe = turretRecipe;

        foreach (TurretButton turretButton in turretButtons)
        {
            turretButton.Highlight(index);
        }
    }

    public void HandlePointInput(InputAction.CallbackContext context)
    {
        Vector2 mouseInput = context.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mouseInput);
    }

    public void HandleClickInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        PlaceTurret();
    }
}

public class AmountToSlot
{
    public int Amount;
    public ItemSlot ItemSlot { get; private set; }

    public AmountToSlot(int amount, ItemSlot itemSlot)
    {
        Amount = amount;
        ItemSlot = itemSlot;
    }
}