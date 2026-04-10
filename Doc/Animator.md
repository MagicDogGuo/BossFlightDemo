# Animator Parameters 清單

## Parameters

| 參數名稱 | 類型 | 使用時機 |
|----------|------|----------|
| `IsMoving` | Bool | `true` = 有移動輸入，`false` = 靜止 |
| `Attack` | Trigger | 進入 AttackState 時觸發 |
| `Dodge` | Trigger | 進入 DodgeState 時觸發 |
| `Parry` | Trigger | 進入 ParryState 時觸發 |
| `Hit` | Trigger | 進入 HitState 時觸發 |
| `Dead` | Trigger | 進入 DeadState 時觸發 |

---

## Animator State 建議連線方式

```
Any State ──[Attack Trigger]──► Attack ──► Idle
Any State ──[Dodge  Trigger]──► Dodge  ──► Idle
Any State ──[Parry  Trigger]──► Parry  ──► Idle
Any State ──[Hit    Trigger]──► Hit    ──► Idle
Any State ──[Dead   Trigger]──► Dead   (no exit)

Idle ──[IsMoving = true ]──► Walk/Run
Walk ──[IsMoving = false]──► Idle
```

---

## 重點注意

- `Attack / Dodge / Parry / Hit / Dead` 全部從 **Any State** 出發，這樣不管當前在哪個 State 都能被打斷
- Transition 的 **Has Exit Time** 建議全部取消勾選，改由 State 計時器（`_timer`）控制持續時間，避免動畫長度與邏輯時間不同步
- `Dead` 的出口 Transition 不需要設，讓它停在最後一幀即可
