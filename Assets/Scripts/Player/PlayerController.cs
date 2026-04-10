using UnityEngine;
using UnityEngine.InputSystem;
using BossFlightDemo.Core;
using BossFlightDemo.Core.StateMachine;
using BossFlightDemo.Player.States;

namespace BossFlightDemo.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // ── Component references / 元件參考 ──────────────
        public CharacterController CharacterController { get; private set; }
        public Animator Animator { get; private set; }

        // ── State machine / 狀態機 ───────────────────────
        public StateMachine<PlayerController> StateMachine { get; private set; }

        // ── Movement settings / 移動參數 ─────────────────
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private float gravity = -20f;

        public float MoveSpeed => moveSpeed;
        public float RotationSpeed => rotationSpeed;

        // ── Attack settings / 攻擊參數 ───────────────────
        [Header("Attack")]
        [SerializeField] private float attackDuration = 0.5f;
        [SerializeField] private float attackCooldown = 0.3f;

        public float AttackDuration => attackDuration;
        public float AttackCooldown => attackCooldown;

        // ── Dodge settings / 閃避參數 ────────────────────
        [Header("Dodge")]
        [SerializeField] private float dodgeSpeed = 12f;
        [SerializeField] private float dodgeDuration = 0.4f;

        public float DodgeSpeed => dodgeSpeed;
        public float DodgeDuration => dodgeDuration;

        // ── Parry settings / 招架參數 ────────────────────
        [Header("Parry")]
        [SerializeField] private float parryWindow = 0.25f;   // Successful parry judgement window / 成功招架的判定窗口
        [SerializeField] private float parryDuration = 0.5f;

        public float ParryWindow => parryWindow;
        public float ParryDuration => parryDuration;
        public bool IsParryActive { get; set; }

        // ── Input cache written by Input System callbacks / 輸入快取 ──
        public Vector2 MoveInput { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool DodgePressed { get; private set; }
        public bool ParryPressed { get; private set; }

        // ── Runtime data / 運行時資料 ────────────────────
        public Vector3 Velocity { get; set; }
        public bool IsDead { get; private set; }

        // ── Pre-allocated state instances — avoids GC per transition / 預建 State 實例
        public PlayerIdleState IdleState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerDodgeState DodgeState { get; private set; }
        public PlayerParryState ParryState { get; private set; }
        public PlayerHitState HitState { get; private set; }
        public PlayerDeadState DeadState { get; private set; }

        // ─────────────────────────────────────────────────

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            Animator = GetComponentInChildren<Animator>();

            IdleState   = new PlayerIdleState();
            AttackState = new PlayerAttackState();
            DodgeState  = new PlayerDodgeState();
            ParryState  = new PlayerParryState();
            HitState    = new PlayerHitState();
            DeadState   = new PlayerDeadState();

            StateMachine = new StateMachine<PlayerController>(this);
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            ApplyGravity();
            StateMachine.Update();
            ConsumeOneFrameInputs();
        }

        // ── Gravity / 重力 ───────────────────────────────
        private void ApplyGravity()
        {
            if (CharacterController.isGrounded && Velocity.y < 0f)
                Velocity = new Vector3(Velocity.x, -2f, Velocity.z);

            Velocity += Vector3.up * gravity * Time.deltaTime;
            CharacterController.Move(new Vector3(0f, Velocity.y, 0f) * Time.deltaTime);
        }

        /// <summary>
        /// Clear one-frame inputs after each tick to prevent cross-frame ghost presses / 每幀末清除單幀輸入
        /// </summary>
        private void ConsumeOneFrameInputs()
        {
            AttackPressed = false;
            DodgePressed  = false;
            ParryPressed  = false;
        }

        // ── Input System callbacks / 輸入回調 ────────────
        public void OnMove(InputValue value)   => MoveInput     = value.Get<Vector2>();
        public void OnAttack(InputValue value) => AttackPressed = value.isPressed;
        public void OnDodge(InputValue value)  => DodgePressed  = value.isPressed;
        public void OnParry(InputValue value)  => ParryPressed  = value.isPressed;

        // ── Public API / 公開 API ────────────────────────
        /// <summary>
        /// Called by incoming attacks — check IsParryActive before calling / 受到攻擊時呼叫，呼叫前需先檢查 IsParryActive
        /// </summary>
        public void TakeHit(int remainingHp)
        {
            if (IsDead) return;
            EventBus.RaisePlayerHit(remainingHp);
            StateMachine.ChangeState(HitState);
        }

        public void Die()
        {
            if (IsDead) return;
            IsDead = true;
            EventBus.RaisePlayerDead();
            StateMachine.ChangeState(DeadState);
        }
    }
}
