using UnityEngine;

public class ShipmentOption : ItemSlot
{
    [SerializeField] AudioClip clickAudioClip;

    bool isActive;

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
        isActive = true;
    }

    public void OnClick()
    {
        if (!isActive) return;
        isActive = false;
        SoundEffectsManager.Instance.PlaySoundEffect(clickAudioClip, transform.position, 1);
        BattleManager.Instance.InvokeOnShipmentCollectedEvent(item, Amount);        
    }
}
