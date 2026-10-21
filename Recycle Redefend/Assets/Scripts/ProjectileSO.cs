using UnityEngine;

[CreateAssetMenu(menuName = "Projectile")]
public class ProjectileSO : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float MaxHeight { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField] public float DamageRadius { get; private set; }
    [field: SerializeField] public int PenetrationCount { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public AnimationCurve TrajectoryCurve { get; private set; }
}
