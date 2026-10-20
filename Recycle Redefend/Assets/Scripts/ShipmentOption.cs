using UnityEngine;

public class ShipmentOption : ItemSlot
{
    ItemSO item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize(ItemSO item, int amount)
    {
        base.Initialize(item, amount);
        this.item = item;
    }

    public void OnClick()
    {
        BattleManager.Instance.InvokeOnShipmentCollectedEvent(item, Amount);        
    }
}
