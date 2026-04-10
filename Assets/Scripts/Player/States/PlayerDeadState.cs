namespace BossFlightDemo.Player.States
{
    public class PlayerDeadState : PlayerBaseState
    {
        public override void Enter(PlayerController owner)
        {
            owner.Animator?.SetTrigger("Dead");
            // TODO: fire EventBus.OnPlayerDead once EventBus is implemented (Day 1) / 待 EventBus 實作後接入
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
