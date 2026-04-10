using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerHitState : PlayerBaseState
    {
        private const float HitStunDuration = 0.4f;
        private float _timer;

        public override void Enter(PlayerController owner)
        {
            _timer = 0f;
            owner.Animator?.SetTrigger("Hit");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            if (_timer >= HitStunDuration)
                owner.StateMachine.ChangeState(owner.IdleState);
        }

        public override void Exit(PlayerController owner)
        {
            _timer = 0f;
        }
    }
}
