using System;
using UnityEngine;

namespace BossFlightDemo.Core
{
    /// <summary>
    /// Static event bus — decouples systems via delegates, no Inspector wiring needed / 靜態事件匯流排，以 delegate 解耦各系統
    /// </summary>
    public static class EventBus
    {
        // ── Player events / 玩家事件 ──────────────────────

        /// <summary>
        /// Fired when the player takes a hit / 玩家受擊時觸發
        /// </summary>
        public static event Action<int> OnPlayerHit;    // int = remaining HP / 剩餘血量

        /// <summary>
        /// Fired when the player dies / 玩家死亡時觸發
        /// </summary>
        public static event Action OnPlayerDead;

        /// <summary>
        /// Fired on a successful parry / 成功招架時觸發
        /// </summary>
        public static event Action OnParrySuccess;

        // ── Boss events / Boss 事件 ───────────────────────

        /// <summary>
        /// Fired when the boss transitions to a new phase / Boss 切換 Phase 時觸發
        /// </summary>
        public static event Action<int> OnBossPhaseChange;  // int = new phase index / 新 Phase 編號

        /// <summary>
        /// Fired when the boss takes a hit / Boss 受擊時觸發
        /// </summary>
        public static event Action<int> OnBossHit;          // int = remaining HP / 剩餘血量

        /// <summary>
        /// Fired when the boss dies / Boss 死亡時觸發
        /// </summary>
        public static event Action OnBossDead;

        // ── Raise helpers / 觸發方法 ─────────────────────

        public static void RaisePlayerHit(int remainingHp)
        {
            OnPlayerHit?.Invoke(remainingHp);
            Log(nameof(OnPlayerHit), remainingHp.ToString());
        }

        public static void RaisePlayerDead()
        {
            OnPlayerDead?.Invoke();
            Log(nameof(OnPlayerDead));
        }

        public static void RaiseParrySuccess()
        {
            OnParrySuccess?.Invoke();
            Log(nameof(OnParrySuccess));
        }

        public static void RaiseBossPhaseChange(int newPhase)
        {
            OnBossPhaseChange?.Invoke(newPhase);
            Log(nameof(OnBossPhaseChange), newPhase.ToString());
        }

        public static void RaiseBossHit(int remainingHp)
        {
            OnBossHit?.Invoke(remainingHp);
            Log(nameof(OnBossHit), remainingHp.ToString());
        }

        public static void RaiseBossDead()
        {
            OnBossDead?.Invoke();
            Log(nameof(OnBossDead));
        }

        // ── Scene cleanup / 場景清理 ──────────────────────

        /// <summary>
        /// Clear all subscribers — call this on scene unload to prevent stale references / 場景卸載時清除所有訂閱，防止殘留參考
        /// </summary>
        public static void ClearAllSubscribers()
        {
            OnPlayerHit       = null;
            OnPlayerDead      = null;
            OnParrySuccess    = null;
            OnBossPhaseChange = null;
            OnBossHit         = null;
            OnBossDead        = null;
        }

        // ── Internal debug log / 內部 debug 輸出 ─────────
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private static void Log(string eventName, string payload = "")
        {
            string msg = string.IsNullOrEmpty(payload)
                ? $"[EventBus] {eventName}"
                : $"[EventBus] {eventName} → {payload}";
            Debug.Log(msg);
        }
    }
}
