# Boss Fight Demo — Unity 3D 開發計畫

> **目標**：以 5 天完成一個具說服力的 3D Boss 戰鬥 Demo，展示系統架構、物理引擎與程式設計模式能力，作為紐西蘭遊戲工程師面試作品。

---

## 遊戲概念

**類型**：3D 單人 Boss 戰（參考：炎姬 Homura Hime 精神原型）

**場景**：圓形競技場（幾何體美術，無需美術資源）

**核心玩法**：
- 玩家：移動 + 近戰攻擊 + 閃避 + **Parry（招架）**
- Boss Phase 1：衝刺攻擊 + 簡單近戰
- Boss Phase 2（50% HP）：子彈地獄 Pattern + 新招式切換

---

## 核心系統架構

### 1. 玩家狀態機 `StateMachine<T>`

**展示重點**：泛型狀態機設計、每個 State 獨立管理 Enter / Execute / Exit

```
Idle → Attack → Dodge → Parry → Hit → Dead
```

使用 `enum PlayerState` + 泛型 `StateMachine<T>` 管理，易於擴充且低耦合。

---

### 2. Boss AI 行為樹

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

### 3. 子彈地獄系統（Bullet Hell）

**展示重點**：Object Pool、資料驅動設計

- **`ProjectilePool`**：管理 Projectile 物件回收，避免 GC 壓力
- **`BulletPattern` ScriptableObject**：螺旋、扇形、追蹤等 Pattern 資料化，設計師可直接在 Inspector 調整

```csharp
// 面試說明：為什麼用 Object Pool？
// 子彈地獄每秒可能生成數百個物件，頻繁 Instantiate/Destroy 
// 造成 GC Spike，Object Pool 可大幅降低記憶體壓力。
```

---

### 4. EventBus（事件匯流排）

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
- [ ] 玩家移動、攻擊、閃避基本手感
- [ ] 圓形競技場場景搭建

---

### Day 2 — Parry 系統 + Boss Phase 1

- [ ] Parry timing window（iFrame + 命中判定）
- [ ] Boss AI：衝刺攻擊 + 簡單近戰行為
- [ ] HP 系統 + Phase Gate 事件觸發

---

### Day 3 — 子彈地獄 + Boss Phase 2

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
        ├── PlayerAttackState.cs
        ├── PlayerDodgeState.cs
        ├── PlayerParryState.cs
        ├── PlayerHitState.cs
        └── PlayerDeadState.cs

```