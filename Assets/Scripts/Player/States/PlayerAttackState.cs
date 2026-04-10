using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerAttackState : PlayerBaseState
    {
        private float _timer;
        private bool  _hitboxEnabled;
        private bool  _exitReady;

        public override void Enter(PlayerController owner)
        {
            _timer         = 0f;
            _hitboxEnabled = false;
            _exitReady     = false;
            owner.Animator?.SetTrigger("Attack");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            // After windup delay, open the hitbox / 前搖結束後開啟判定框
            if (!_hitboxEnabled && _timer >= owner.AttackWindup)
            {
                _hitboxEnabled = true;
                owner.AttackHitbox?.EnableHitbox(owner.AttackDamage);
            }

            // After active duration, close the hitbox / 判定時間結束後關閉判定框
            if (_hitboxEnabled && _timer >= owner.AttackDuration)
            {
                _hitboxEnabled = false;
                _exitReady     = true;
                owner.AttackHitbox?.DisableHitbox();
            }

            // Return to Idle after full duration + cooldown / 動作結束 + 冷卻後回 Idle
            if (_exitReady && _timer >= owner.AttackDuration + owner.AttackCooldown)
                owner.StateMachine.ChangeState(owner.IdleState);
        }

        public override void Exit(PlayerController owner)
        {
            // Safety — ensure hitbox is off if state is interrupted / 安全保護：狀態被中斷時確保判定框關閉
            if (_hitboxEnabled)
                owner.AttackHitbox?.DisableHitbox();

            _timer         = 0f;
            _hitboxEnabled = false;
        }
    }
}
