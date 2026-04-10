using UnityEngine;

namespace BossFlightDemo.Core.StateMachine
{
    /// <summary>
    /// Generic state machine — centralises transitions, states stay decoupled / 泛型狀態機，集中管理切換
    /// </summary>
    public class StateMachine<T>
    {
        private readonly T _owner;
        private IState<T> _currentState;
        private IState<T> _previousState;

        public IState<T> CurrentState => _currentState;
        public IState<T> PreviousState => _previousState;

        public StateMachine(T owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Set the initial state and call Enter / 初始化並進入第一個狀態
        /// </summary>
        public void Initialize(IState<T> initialState)
        {
            _currentState = initialState;
            _currentState.Enter(_owner);
        }

        /// <summary>
        /// Tick the current state every frame / 每幀驅動當前狀態
        /// </summary>
        public void Update()
        {
            _currentState?.Execute(_owner);
        }

        /// <summary>
        /// Exit the old state then enter the new one / 先 Exit 舊狀態再 Enter 新狀態
        /// </summary>
        public void ChangeState(IState<T> newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("[StateMachine] ChangeState: newState is null.");
                return;
            }

            _previousState = _currentState;
            _currentState?.Exit(_owner);
            _currentState = newState;
            _currentState.Enter(_owner);
        }

        /// <summary>
        /// Return to the previous state, e.g. after Hit / 回到上一個狀態
        /// </summary>
        public void RevertToPreviousState()
        {
            if (_previousState != null)
                ChangeState(_previousState);
        }
    }
}
