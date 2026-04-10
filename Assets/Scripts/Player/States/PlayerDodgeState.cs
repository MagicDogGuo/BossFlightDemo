using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerDodgeState : PlayerBaseState
    {
        private float _timer;
        private Vector3 _dodgeDirection;

        public override void Enter(PlayerController owner)
        {
            _timer = 0f;

            // Dodge in the input direction; fall back to backward roll if no input / 依輸入方向閃避，無輸入則向後滾
            var input = owner.MoveInput;
            if (input.sqrMagnitude > 0.01f)
            {
                var cam = Camera.main;
                Vector3 forward = cam != null
                    ? Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized
                    : Vector3.forward;
                Vector3 right = cam != null
                    ? Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized
                    : Vector3.right;

                _dodgeDirection = (forward * input.y + right * input.x).normalized;
            }
            else
            {
                _dodgeDirection = -owner.transform.forward;
            }

            owner.Animator?.SetTrigger("Dodge");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            owner.CharacterController.Move(_dodgeDirection * owner.DodgeSpeed * Time.deltaTime);

            if (_timer >= owner.DodgeDuration)
                owner.StateMachine.ChangeState(owner.IdleState);
        }

        public override void Exit(PlayerController owner)
        {
            _timer = 0f;
        }
    }
}
