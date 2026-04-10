using UnityEngine;

namespace BossFlightDemo.Player
{
    /// <summary>
    /// Data-driven definition for a single combo hit.
    /// Create via Assets > Create > BossFlightDemo > AttackData.
    /// </summary>
    [CreateAssetMenu(menuName = "BossFlightDemo/AttackData", fileName = "AttackData")]
    public class AttackData : ScriptableObject
    {
        [Header("Damage")]
        [Tooltip("Multiplier applied to PlayerController.AttackDamage  (1.0×/1.3×/2.0×)")]
        [Min(0.1f)] public float damageMultiplier = 1f;

        [Header("Timing (seconds)")]
        [Tooltip("Delay before the hitbox opens — the 'wind-up' phase")]
        [Min(0f)] public float windup = 0.12f;

        [Tooltip("How long the hitbox stays active")]
        [Min(0.01f)] public float activeDuration = 0.20f;

        [Tooltip("Recovery window after hitbox closes — player can buffer the next attack here")]
        [Min(0.05f)] public float timingWindow = 0.35f;
    }
}
