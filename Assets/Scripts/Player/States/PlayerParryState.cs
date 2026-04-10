using UnityEngine;

namespace BossFlightDemo.Player.States
{
    /// <summary>
    /// Parry state — hits received within ParryWindow count as a successful parry / 招架狀態，時間窗口內受擊視為成功
    /// </summary>
    public class PlayerParryState : PlayerBaseState
    {
        private float _timer;

        public override void Enter(PlayerController owner)
        {
            _timer = 0f;
            owner.IsParryActive = true;    // Checked by incoming attacks to detect a successful parry / 外部攻擊命中時檢查此旗標
            owner.Animator?.SetTrigger("Parry");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            // Disable parry judgement after the window closes / ParryWindow 結束後關閉判定
            if (_timer >= owner.ParryWindow)
                owner.IsParryActive = false;

            // Return to Idle once the full parry animation finishes / 整體動作結束後回 Idle
            if (_timer >= owner.ParryDuration)
                owner.StateMachine.ChangeState(owner.IdleState);
        }

        public override void Exit(PlayerController owner)
        {
            owner.IsParryActive = false;
            _timer = 0f;
        }
    }
}
