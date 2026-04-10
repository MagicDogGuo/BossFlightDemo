namespace BossFlightDemo.Core.StateMachine
{
    /// <summary>
    /// Generic state interface — each state owns its lifecycle / 泛型狀態介面，每個 State 自管生命週期
    /// </summary>
    public interface IState<T>
    {
        void Enter(T owner);
        void Execute(T owner);
        void Exit(T owner);
    }
}
