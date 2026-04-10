namespace BossFlightDemo.Player.States
{
    public class PlayerDeadState : PlayerBaseState
    {
        public override void Enter(PlayerController owner)
        {
            owner.Animator?.SetTrigger("Dead");
            // PlayerController.Die() already raises OnPlayerDead before entering this state / Die() 進入此 State 前已觸發事件
        }

        public override void Execute(PlayerController owner)
        {
            // Dead state ignores all input / 死亡狀態不處理任何輸入
        }

        public override void Exit(PlayerController owner)
        {
            // Should never exit Dead except on scene reset / 僅場景重置時才離開此狀態
        }
    }
}
