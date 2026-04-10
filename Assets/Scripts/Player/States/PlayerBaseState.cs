using BossFlightDemo.Core.StateMachine;

namespace BossFlightDemo.Player.States
{
    /// <summary>
    /// Abstract base for all player states — provides shared helper methods / 所有玩家狀態的抽象基底
    /// </summary>
    public abstract class PlayerBaseState : IState<PlayerController>
    {
        public abstract void Enter(PlayerController owner);
        public abstract void Execute(PlayerController owner);
        public abstract void Exit(PlayerController owner);

        /// <summary>
        /// Move and rotate the character based on current input / 依輸入移動並旋轉面向方向
        /// </summary>
        protected void HandleMovement(PlayerController owner)
        {
            var input = owner.MoveInput;
            if (input.sqrMagnitude < 0.01f) return;

            var cam = UnityEngine.Camera.main;
            UnityEngine.Vector3 forward = cam != null
                ? UnityEngine.Vector3.ProjectOnPlane(cam.transform.forward, UnityEngine.Vector3.up).normalized
                : UnityEngine.Vector3.forward;
            UnityEngine.Vector3 right = cam != null
                ? UnityEngine.Vector3.ProjectOnPlane(cam.transform.right, UnityEngine.Vector3.up).normalized
                : UnityEngine.Vector3.right;

            UnityEngine.Vector3 moveDir = (forward * input.y + right * input.x).normalized;
            owner.CharacterController.Move(moveDir * owner.MoveSpeed * UnityEngine.Time.deltaTime);

            // 平滑旋轉面向移動方向
            UnityEngine.Quaternion targetRot = UnityEngine.Quaternion.LookRotation(moveDir);
            owner.transform.rotation = UnityEngine.Quaternion.RotateTowards(
                owner.transform.rotation, targetRot,
                owner.RotationSpeed * UnityEngine.Time.deltaTime);
        }
    }
}
