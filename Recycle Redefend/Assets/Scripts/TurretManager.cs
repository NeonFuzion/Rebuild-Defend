using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TurretManager : MonoBehaviour
{
    [SerializeField] float recycleRatio = 0.7f, turrentPlacementDistance = 2;
    [SerializeField] Vector2 placementBounds, baseBounds = new Vector2(2, 2), durabilityBarOffset = new Vector2(0, -1.25f);
    [SerializeField] DurabilityBar prefabDurabilityBar;
    [SerializeField] ItemSlot prefabItemSlot;
    [SerializeField] TurretButton prefabTurretButton;
    [SerializeField] AudioClip placeSuccessAudioClip, placeFailAudioClip;
    [SerializeField] ItemSO unrecyclable;
    [SerializeField] Transform itemParent, turretParent;
    [SerializeField] TurretRecipeSO[] turretRecipes;
    [SerializeField] ItemToAmount[] items;
    [SerializeField] UnityEvent onTurretPlaced;

    Vector2 mousePosition;
    TurretRecipeSO currentRecipe;

    TurretButton[] turretButtons;
    List<Vector2> turretPositions;
    Dictionary<ItemSO, AmountToSlot> inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleManager.Instance.AddOnShipmentCollectedListener((object obj, BattleManager.AddItemArgs args) =>
            AddItem(args.Item, args.Amount));
        BattleManager.Instance.AddOnTurretRecycledListener((object obj, BattleManager.TurretRecycleArgs args) =>
            RecycleTurret(args.TurretRecipe, args.Position));
        BattleManager.Instance.AddOnTurretHoverEnterListener((object obj, int args) => ShowCost(turretRecipes[args]));
        BattleManager.Instance.AddOnTurretHoverExitListener((object obj, EventArgs args) => HideCost());

        inventory = new Dictionary<ItemSO, AmountToSlot>();
        foreach (ItemToAmount pair in items)
        {
            ItemSlot itemSlot = Instantiate(prefabItemSlot, itemParent);
            itemSlot.Initialize(pair.Item, pair.Amount);
            inventory.Add(pair.Item, new (pair.Amount, itemSlot));
        }

        turretButtons = new TurretButton[turretRecipes.Length];
        for (int i = 0; i < turretRecipes.Length; i++)
        {
            TurretButton turretButton = Instantiate(prefabTurretButton, turretParent);
            turretButtons[i] = turretButton;
            turretButton.Initialize(i, turretRecipes[i], SetTurret);
        }

        turretPositions = new List<Vector2>();
        SetTurret(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceTurret()
    {
        if (!currentRecipe || BattleManager.Instance.IsInCombat ||
            turretPositions.Find(pos => Vector2.Distance(pos, mousePosition) < turrentPlacementDistance) != Vector2.zero ||
            Mathf.Abs(mousePosition.x) > placementBounds.x || Mathf.Abs(mousePosition.y) > placementBounds.y ||
            (Mathf.Abs(mousePosition.x) < baseBounds.x && Mathf.Abs(mousePosition.y) < baseBounds.y))
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
        turretGameObject.GetComponent<Turret>().Initialize(durabilityBar, currentRecipe);
        SoundEffectsManager.Instance.PlaySoundEffect(placeSuccessAudioClip, mousePosition, 1);
        turretPositions.Add(turretGameObject.transform.position);
        onTurretPlaced?.Invoke();
    }

    void AddItem(ItemSO item, int amount)
    {
        AmountToSlot amountToSlot = inventory[item];
        amountToSlot.Amount += amount;
        amountToSlot.ItemSlot.UpdateAmount(amountToSlot.Amount);
    }

    void RecycleTurret(TurretRecipeSO turretRecipe, Vector2 position)
    {
        turretPositions.Remove(position);

        foreach (ItemToAmount pair in turretRecipe.Materials)
        {
            if (pair.Item == unrecyclable) return;
        }

        foreach (ItemToAmount pair in turretRecipe.Materials)
        {
            AddItem(pair.Item, Mathf.RoundToInt(pair.Amount * recycleRatio));
        }
    }

    void SetTurret(int index)
    {
        currentRecipe = turretRecipes[index];

        foreach (TurretButton turretButton in turretButtons)
        {
            turretButton.Highlight(index);
        }
    }

    void ShowCost(TurretRecipeSO currentRecipe)
    {
        foreach (ItemToAmount pair in currentRecipe.Materials)
        {
            inventory[pair.Item].ItemSlot.SetCost(pair.Amount);
        }
        
        if (currentRecipe.Materials.Length == 4) return;
        inventory[unrecyclable].ItemSlot.SetCost();
    }

    void HideCost()
    {
        if (currentRecipe)
        {
            ShowCost(currentRecipe);
        }
        else
        {
            foreach (AmountToSlot pair in inventory.Values)
            {
                pair.ItemSlot.SetCost();
            }
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