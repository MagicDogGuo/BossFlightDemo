# 攻擊時間軸

Enter()
  │
  ├─ 0.00s  動畫 Trigger 觸發（前搖開始）
  │
  ├─ 0.12s  [AttackWindup] → EnableHitbox()  ← 判定框開啟
  │
  ├─ 0.35s  [AttackDuration] → DisableHitbox()  ← 判定框關閉
  │
  └─ 0.60s  [AttackDuration + Cooldown] → 回 Idle

  關聯檔案: PlayerController.cs、PlayerAttackState.cs 、IDamageable.cs 、AttackHitbox.cs



# iFrame 時間軸

Enter Dodge
  │
  ├─ 0.00s  IsInvincible = true  ← 無敵開始
  │          角色開始位移
  │
  ├─ 0.30s  [IFrameDuration] IsInvincible = false  ← 無敵結束（還在滑行）
  │
  └─ 0.40s  [DodgeDuration] → 回 Idle

關聯檔案: PlayerController.cs、PlayerDodgeState.cs、
