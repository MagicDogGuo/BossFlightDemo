using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerDodgeState : PlayerBaseState
    {
        private float   _timer;
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

            // Activate iFrames at the start of the dodge / 閃避開始時啟動無敵幀
            owner.IsInvincible = true;
            owner.Animator?.SetTrigger("Dodge");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            owner.CharacterController.Move(_dodgeDirection * owner.DodgeSpeed * Time.deltaTime);

            // iFrame ends after IFrameDuration; character is still sliding but can be hit / 無敵時間結束，角色仍在移動但可被攻擊
            if (owner.IsInvincible && _timer >= owner.IFrameDuration)
                owner.IsInvincible = false;

            if (_timer >= owner.DodgeDuration)
                owner.StateMachine.ChangeState(owner.IdleState);
        }

        public override void Exit(PlayerController owner)
        {
            // Safety — always clear iFrames on exit / 安全保護：離開狀態時確保無敵旗標清除
            owner.IsInvincible = false;
            _timer = 0f;
        }
    }
}
