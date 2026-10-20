using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TurretManager : MonoBehaviour
{
    [SerializeField] int itemCount = 10;
    [SerializeField] ItemSlot prefabItemSlot;
    [SerializeField] Transform itemParent;
    [SerializeField] AudioClip placeSuccessAudioClip, placeFailAudioClip;
    [SerializeField] ItemSO[] items;
    [SerializeField] UnityEvent onTurretPlaced;

    Vector2 mousePosition;
    TurretRecipeSO currentRecipe;

    Dictionary<ItemSO, AmountToSlot> inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<ItemSO, AmountToSlot>();
        foreach (ItemSO item in items)
        {
            ItemSlot itemSlot = Instantiate(prefabItemSlot, itemParent);
            itemSlot.Initialize(item.Sprite, itemCount);
            inventory.Add(item, new (itemCount, itemSlot));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceTurret()
    {
        if (!currentRecipe)
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
        SoundEffectsManager.Instance.PlaySoundEffect(placeSuccessAudioClip, mousePosition, 1);
        onTurretPlaced?.Invoke();
    }

    public void SetTurret(TurretRecipeSO turretRecipe)
    {
        currentRecipe = turretRecipe;
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