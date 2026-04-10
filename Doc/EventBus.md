# EventBus 設計摘要

| 事件 | 方法 | 參數 | 訂閱者（之後實作） |
|------|------|------|-------------------|
| `OnPlayerHit` | `RaisePlayerHit(int hp)` | 剩餘血量 | HUD、CameraShake、AudioManager |
| `OnPlayerDead` | `RaisePlayerDead()` | — | GameManager、UI |
| `OnParrySuccess` | `RaiseParrySuccess()` | — | ParticleSystem、AudioManager |
| `OnBossPhaseChange` | `RaiseBossPhaseChange(int phase)` | Phase 編號 | BossAI、UI、CinematicManager |
| `OnBossHit` | `RaiseBossHit(int hp)` | 剩餘血量 | HUD |
| `OnBossDead` | `RaiseBossDead()` | — | GameManager、UI |
