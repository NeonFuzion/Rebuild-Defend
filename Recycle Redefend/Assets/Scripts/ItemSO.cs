using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemSO : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
}
