# Animator 設定

### Parameters 清單

| 參數名稱 | 類型 | 使用時機 |
|----------|------|----------|
| `IsMoving` | Bool | 有移動輸入時為 true |
| `Attack` | Trigger | 進入下一段攻擊時觸發（先設 ComboIndex 再觸發）|
| `ComboIndex` | Int | 0 = 第一擊、1 = 第二擊、2 = 第三擊 |
| `BackToIdle` | Trigger | ComboState 結束時觸發（沒輸入逾時 or 第三擊打完）|
| `Dodge` | Trigger | 進入 DodgeState 時觸發 |
| `Parry` | Trigger | 進入 ParryState 時觸發 |
| `Hit` | Trigger | 進入 HitState 時觸發 |
| `Dead` | Trigger | 進入 DeadState 時觸發 |

---

### Animator State 連線方式

```
Idle ──[Attack + ComboIndex=0]──► Attack1 ──┬──[Attack + ComboIndex=1]──► Attack2 ──┬──[Attack + ComboIndex=2]──► Attack3 ──[BackToIdle]──► Idle
                                             └──[BackToIdle]──► Idle                 └──[BackToIdle]──► Idle

Idle ──[IsMoving = true ]──► Walk
Walk ──[IsMoving = false]──► Idle

Any State ──[Dodge Trigger]──► Dodge ──► Idle
Any State ──[Parry Trigger]──► Parry ──► Idle
Any State ──[Hit   Trigger]──► Hit   ──► Idle
Any State ──[Dead  Trigger]──► Dead  (no exit)
```

---

### 三連擊的連線邏輯說明

三連擊**不從 Any State 出發**，改由各擊之間互相銜接，每一段都有兩條出口：

- **有輸入（timing window 內按攻擊）** → 觸發 `Attack` + 下一個 `ComboIndex` → 進入下一擊
- **沒輸入（timing window 逾時）or 第三擊結束** → 觸發 `BackToIdle` → 回到 Idle

`Dodge / Parry / Hit / Dead` 繼續保留 Any State，這些才是真正需要隨時打斷的狀態。

程式碼在 `ComboState.Enter()` 中的呼叫順序：

```csharp
_animator.SetInteger("ComboIndex", _comboIndex); // 必須先設 index
_animator.SetTrigger("Attack");                   // 再觸發 trigger
```

程式碼在 `ComboState.Exit()` 中：

```csharp
_animator.SetTrigger("BackToIdle"); // timing window 逾時或第三擊結束都走這裡
```

> ⚠️ `SetInteger` 必須在 `SetTrigger("Attack")` 之前：Trigger 觸發後 Animator 立刻評估 Transition 條件，若此時 ComboIndex 還未更新，會跳到錯誤的攻擊段。

---

### Transition 設定重點

- **Has Exit Time**：攻擊 → 下一擊、攻擊 → Idle 的 Transition 全部取消勾選，改由程式碼 `_timer` 控制
- **Transition Duration**：設為 0，避免攻擊動畫之間有淡入淡出模糊感
- **BackToIdle Transition**：Priority 低於 Attack Transition，確保有輸入時優先銜接下一擊而非回 Idle
- **Dead** 的出口 Transition 不需要設，讓它停在最後一幀

---

### Transition 設定重點

- **Has Exit Time**：全部取消勾選，由程式碼 `_timer` 控制持續時間
- **Transition Duration**：設為 0，避免攻擊動畫之間有淡入淡出模糊感
- **Dead** 的出口 Transition 不需要設，讓它停在最後一幀

---