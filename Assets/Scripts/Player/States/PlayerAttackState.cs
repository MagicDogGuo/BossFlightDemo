using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerAttackState : PlayerBaseState
    {
        private float _timer;
        private bool _exitReady;

        public override void Enter(PlayerController owner)
        {
            _timer     = 0f;
            _exitReady = false;
            owner.Animator?.SetTrigger("Attack");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            // Wait for Cooldown after the attack animation ends before returning to Idle / 動作結束後等待冷卻才回 Idle
            if (!_exitReady && _timer >= owner.AttackDuration)
                _exitReady = true;

            if (_exitReady && _timer >= owner.AttackDuration + owner.AttackCooldown)
            {
                owner.StateMachine.ChangeState(owner.IdleState);
            }
        }

        public override void Exit(PlayerController owner)
        {
            _timer = 0f;
        }
    }
}
