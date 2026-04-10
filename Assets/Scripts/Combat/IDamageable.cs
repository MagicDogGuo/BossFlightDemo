namespace BossFlightDemo.Combat
{
    /// <summary>
    /// Implemented by anything that can receive damage / 任何可受傷的物件實作此介面
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage and return remaining HP / 扣血並回傳剩餘血量
        /// </summary>
        int TakeDamage(int amount);

        bool IsDead { get; }
    }
}
