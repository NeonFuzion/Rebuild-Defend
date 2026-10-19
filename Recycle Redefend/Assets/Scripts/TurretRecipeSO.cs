using UnityEngine;

[CreateAssetMenu(menuName = "Turret Recipe")]
public class TurretRecipeSO : ScriptableObject
{
    [field: SerializeField] public GameObject PrefabTurret { get; private set; }
    [field: SerializeField] public Sprite BaseSprite { get; private set; }
    [field: SerializeField] public Sprite PivotSprite { get; private set; }
    [field: SerializeField] public ItemToAmount[] Materials { get; private set; }
}

[System.Serializable]
public class ItemToAmount
{
    [field: SerializeField] public ItemSO Item { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
}