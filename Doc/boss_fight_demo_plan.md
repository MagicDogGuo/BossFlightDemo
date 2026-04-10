# Boss Fight Demo — Unity 3D 開發計畫

> **目標**：以 5 天完成一個具說服力的 3D Boss 戰鬥 Demo，展示系統架構、物理引擎與程式設計模式能力，作為紐西蘭遊戲工程師面試作品。

---
## 遊戲概念

**類型**：3D 單人 Boss 戰（參考：炎姬 Homura Hime 精神原型）

**場景**：圓形競技場（幾何體美術，無需美術資源）

**核心玩法**：
- 玩家：移動 + **三連擊系統** + 閃避 + Parry（招架）
- Boss Phase 1：衝刺攻擊 + 簡單近戰
- Boss Phase 2（50% HP）：子彈地獄 Pattern + 新招式切換

---

## 核心系統架構

### 1. 玩家狀態機 `StateMachine<T>`

**展示重點**：泛型狀態機設計、每個 State 獨立管理 Enter / Execute / Exit

```
Idle → Attack1 → Attack2 → Attack3 → Idle
          ↓ (timing window 內再按攻擊鍵)
       Dodge → Parry → Hit → Dead
```

使用 `enum PlayerState` + 泛型 `StateMachine<T>` 管理，易於擴充且低耦合。

---

### 2. 三連擊系統（Combo System）

**展示重點**：timing window 設計、輸入緩衝、傷害遞增邏輯

**連擊規則**：
- 每次按攻擊鍵在 timing window 內觸發下一擊
- 超出 timing window 不輸入 → 自動回到 Idle
- 三擊完成後強制回到 Idle（需重新起手）

**傷害遞增**：

| 段數 | 動作 | 傷害倍率 |
|------|------|----------|
| 第一擊 | 普通橫斬 | 1.0× |
| 第二擊 | 普通直斬 | 1.3× |
| 第三擊 | 重擊下砸 | 2.0× |

**技術實作重點**：
- `ComboState` 持有 `comboIndex`（0、1、2），每次 `Enter()` 時讀取對應攻擊資料
- `AttackData` ScriptableObject 定義各擊的傷害、hitbox 持續時間、timing window 長度
- 輸入緩衝（Input Buffer）：在攻擊動畫結尾前偵測到按鍵即排入下一擊，避免手感卡頓

```csharp
// 面試說明：為什麼要做輸入緩衝？
// 若只在 timing window 開啟的瞬間判斷輸入，玩家稍早按鍵就會漏判，
// 手感會變得很死板。緩衝區在前幾幀就記錄輸入意圖，提升操作流暢度。
```

### 3. Boss AI 行為樹

**展示重點**：Phase 切換架構、攻擊池設計、HP Gate 事件觸發

```
BossAI
├── Phase 1（100% ~ 50% HP）
│   ├── 衝刺攻擊
│   └── 簡單近戰
└── Phase 2（50% HP 以下）
    ├── 子彈地獄 Pattern
    ├── 強化近戰
    └── 新特殊技
```

HP Gate 觸發 `EventBus.OnBossPhaseChange`，驅動演出、音效、UI 同步更新。

---

### 4. 子彈地獄系統（Bullet Hell）

**展示重點**：Object Pool、資料驅動設計

- **`ProjectilePool`**：管理 Projectile 物件回收，避免 GC 壓力
- **`BulletPattern` ScriptableObject**：螺旋、扇形、追蹤等 Pattern 資料化，設計師可直接在 Inspector 調整

```csharp
// 面試說明：為什麼用 Object Pool？
// 子彈地獄每秒可能生成數百個物件，頻繁 Instantiate/Destroy 
// 造成 GC Spike，Object Pool 可大幅降低記憶體壓力。
```

---

### 5. EventBus（事件匯流排）

**展示重點**：觀察者模式、系統解耦

解耦 UI、音效、鏡頭震動等系統，避免各模組互相依賴：

| 事件 | 訂閱者 |
|------|--------|
| `OnPlayerHit` | HUD、CameraShake、AudioManager |
| `OnParrySuccess` | ParticleSystem、AudioManager |
| `OnBossPhaseChange` | BossAI、UI、CinematicManager |
| `OnPlayerDead` | GameManager、UI |

---

## 5 天開發時程

### Day 1 — 架構 + 玩家基礎

- [ ] 建立 `StateMachine<T>` 泛型 + `PlayerStateMachine`
- [ ] `EventBus` 實作（static event + delegate）
- [ ] 玩家移動、閃避基本手感
    1. 相機跟隨 → 移動才有真實手感
    2. AttackHitbox → 攻擊才能真正打到 Boss
    3. iFrame → 閃避才有戰略意義
- [ ] 圓形競技場場景搭建

---

### Day 2 — 三連擊 + Parry 系統

- [ ] `AttackData` ScriptableObject（傷害倍率、hitbox 時間、timing window）
- [ ] `ComboState` 實作（comboIndex 0→1→2 + 輸入緩衝）
- [ ] 三連擊手感調整（Attack1 → Attack2 → Attack3）
- [ ] Parry timing window（iFrame + 命中判定）

---

### Day 3 — Boss Phase 1 + Phase 2

- [ ] Boss AI：衝刺攻擊 + 簡單近戰行為
- [ ] HP 系統 + Phase Gate 事件觸發
- [ ] `ObjectPool` 實作（`ProjectilePool`）
- [ ] `BulletPattern` ScriptableObject（螺旋、扇形、追蹤）
- [ ] Phase 2 Boss 行為切換 + 新動作加入

---

### Day 4 — Juice + UI + 打磨

- [ ] Camera Shake（`OnPlayerHit` / `OnParrySuccess`）
- [ ] Particle Effect（打擊特效、Parry 閃光）
- [ ] HUD：玩家 HP、Boss HP Bar + Phase 指示
- [ ] 開始 / 死亡 / 勝利畫面

---

### Day 5 — 收尾 + 發布

- [ ] Bug 修復 + 難度微調
- [ ] **WebGL Build**（面試官可直接在瀏覽器試玩）
- [ ] GitHub README：架構圖 + 技術決策說明
- [ ] 錄製 30 秒 gameplay clip

---

## 美術方案（零美術資源）

全程使用 Unity 內建幾何體 + Particle System：

| 角色 | 幾何體 |
|------|--------|
| 玩家 | Capsule |
| Boss | 大 Sphere + 附屬 Cube |
| 子彈 | 小 Sphere |
| 打擊感 | Particle System + Camera Shake |

---

## 面試常見問題準備

**Q：你的三連擊系統怎麼設計的？**
> 用 `ComboState` 持有 `comboIndex`，每次 `Enter()` 讀取對應的 `AttackData` ScriptableObject（傷害、hitbox 時間、timing window）。加入輸入緩衝讓玩家在動畫結尾前提早按鍵也能銜接下一擊，避免手感卡頓。

**Q：你為什麼用 Object Pool？**
> 子彈地獄每秒可能生成幾百個物件，頻繁 Instantiate/Destroy 會造成 GC 壓力與畫面卡頓，Object Pool 預先建立物件並重複利用，大幅降低記憶體開銷。

**Q：你的 StateMachine 怎麼設計的？**
> 使用泛型 `StateMachine<T>`，每個 State 自己管理 `Enter()`、`Execute()`、`Exit()`，轉換邏輯集中在 StateMachine 本身，易於擴充且不會互相污染。

**Q：EventBus 和 UnityEvent 有什麼差？**
> UnityEvent 依賴 Inspector 連線，適合設計師操作；EventBus 用 static delegate 實作，跨場景使用更彈性，也更方便單元測試，解耦程度更高。

---

## GitHub README 建議內容

1. **專案簡介** — 一句話說明這是什麼
2. **架構圖** — 各系統關係圖（可用 Mermaid）
3. **技術決策** — 為什麼選擇這些設計模式
4. **WebGL Demo 連結** — 讓面試官直接試玩
5. **已知限制** — 誠實說明哪些沒做完、為什麼

> 💡 README 是第二份作品集——寫清楚「為什麼用 Object Pool」、「StateMachine 的設計考量」比程式碼本身更讓面試官印象深刻。

---

*製作時間：3–5 天 ／ 技術棧：Unity + C# ／ 目標：紐西蘭遊戲工程師面試*


## StateMachine

```
Assets/Scripts/
├── Core/
│   └── StateMachine/
│       ├── IState.cs             ← 泛型介面
│       └── StateMachine.cs       ← 泛型狀態機核心
└── Player/
    ├── PlayerController.cs       ← MonoBehaviour + Input
    └── States/
        ├── PlayerBaseState.cs    ← 抽象基底（含移動輔助）
        ├── PlayerIdleState.cs
        ├── PlayerComboState.cs
        ├── PlayerDodgeState.cs
        ├── PlayerParryState.cs
        ├── PlayerHitState.cs
        └── PlayerDeadState.cs

```