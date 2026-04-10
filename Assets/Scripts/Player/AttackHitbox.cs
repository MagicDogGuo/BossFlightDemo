using System.Collections.Generic;
using UnityEngine;
using BossFlightDemo.Combat;

namespace BossFlightDemo.Player
{
    /// <summary>
    /// Trigger collider that deals damage during the active attack window / 攻擊判定框，在有效視窗內對碰觸到的目標造成傷害
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AttackHitbox : MonoBehaviour
    {
        // ── Runtime / 運行時資料 ──────────────────────────
        private int _damage;

        /// <summary>
        /// Targets already hit this swing — prevents hitting the same target twice / 本次揮擊已命中的目標，防止重複計算
        /// </summary>
        private readonly HashSet<IDamageable> _hitTargets = new HashSet<IDamageable>();

        private Collider _collider;

        // ─────────────────────────────────────────────────

        private void Awake()
        {
            _collider         = GetComponent<Collider>();
            _collider.isTrigger = true;
            gameObject.SetActive(false);    // Start disabled — enabled only during attack window / 預設關閉，僅在攻擊視窗開啟
        }

        // ── Public API / 公開 API ─────────────────────────

        /// <summary>
        /// Activate the hitbox for a new swing / 開始新一次揮擊，啟動判定框
        /// </summary>
        public void EnableHitbox(int damage)
        {
            _damage = damage;
            _hitTargets.Clear();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivate the hitbox at the end of the attack window / 攻擊視窗結束，停用判定框
        /// </summary>
        public void DisableHitbox()
        {
            gameObject.SetActive(false);
            _hitTargets.Clear();
        }

        // ── Collision detection / 碰撞偵測 ───────────────

        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponentInParent<IDamageable>();

            // Skip if target not damageable, already dead, or already hit this swing / 跳過：不可傷害、已死亡、或本次已命中
            if (damageable == null)          return;
            if (damageable.IsDead)           return;
            if (_hitTargets.Contains(damageable)) return;

            _hitTargets.Add(damageable);
            int remaining = damageable.TakeDamage(_damage);

            Debug.Log($"[AttackHitbox] Hit {other.name} — remaining HP: {remaining}");
        }
    }
}
