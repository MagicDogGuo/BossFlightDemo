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

  關聯檔案: PlayerController.cs、PlayerAttackState.cs 
