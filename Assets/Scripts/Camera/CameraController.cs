using UnityEngine;
using UnityEngine.InputSystem;

namespace BossFlightDemo.CameraSystem
{
    /// <summary>
    /// Third-person orbit camera with smooth follow / 第三人稱環繞相機，平滑跟隨玩家
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // ── Target / 跟隨目標 ─────────────────────────────
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.4f, 0f); // Look-at offset, aims at chest height / 注視點偏移，對準胸口高度

        // ── Orbit settings / 環繞參數 ─────────────────────
        [Header("Orbit")]
        [SerializeField] private float distance = 6f;
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float gamepadSensitivity = 90f;     // deg/sec / 每秒旋轉角度
        [SerializeField] private float minPitch = -15f;
        [SerializeField] private float maxPitch = 50f;
        [SerializeField] private float initialPitch = 20f;

        // ── Smooth follow / 平滑跟隨 ──────────────────────
        [Header("Smooth Follow")]
        [SerializeField] private float positionSmoothTime = 0.08f;
        [SerializeField] private float rotationSmoothTime = 0.05f;

        // ── Collision / 碰撞回縮 ──────────────────────────
        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask = ~0;       // Everything by default / 預設偵測所有層
        [SerializeField] private float collisionRadius = 0.2f;

        // ── Runtime / 運行時資料 ──────────────────────────
        private float _yaw;
        private float _pitch;
        private Vector3 _posVelocity;
        private float _currentDistance;

        // ─────────────────────────────────────────────────

        private void Awake()
        {
            _pitch           = initialPitch;
            _currentDistance = distance;

            // Initialise yaw to face the same direction as the target / 初始偏航角對齊目標朝向
            if (target != null)
                _yaw = target.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            ReadInput();
            UpdatePosition();
        }

        // ── Input reading / 讀取輸入 ──────────────────────
        private void ReadInput()
        {
            Vector2 look = Vector2.zero;

            // Mouse delta / 滑鼠位移
            if (Mouse.current != null)
                look += Mouse.current.delta.ReadValue() * mouseSensitivity;

            // Gamepad right stick / 手把右搖桿
            if (Gamepad.current != null)
                look += Gamepad.current.rightStick.ReadValue() * (gamepadSensitivity * Time.deltaTime);

            _yaw   += look.x;
            _pitch -= look.y;   // Invert Y so pushing stick up looks up / 反轉 Y 使上推搖桿對應鏡頭上移
            _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        // ── Position & rotation update / 更新位置與旋轉 ──
        private void UpdatePosition()
        {
            Vector3 pivot = target.position + targetOffset;

            Quaternion orbitRot   = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3    desiredDir = orbitRot * Vector3.back;    // Camera sits behind target / 相機在目標後方

            // Collision — shorten distance if something is in the way / 碰撞偵測：縮短距離避免穿牆
            float targetDist = distance;
            if (Physics.SphereCast(pivot, collisionRadius, desiredDir, out RaycastHit hit,
                                   distance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                targetDist = Mathf.Max(hit.distance - collisionRadius, 0.5f);
            }
            _currentDistance = Mathf.Lerp(_currentDistance, targetDist, Time.deltaTime / rotationSmoothTime);

            Vector3 desiredPos = pivot + desiredDir * _currentDistance;

            // Smooth position / 平滑位置
            transform.position = Vector3.SmoothDamp(
                transform.position, desiredPos, ref _posVelocity, positionSmoothTime);

            // Always look at pivot / 持續注視注視點
            transform.LookAt(pivot);
        }

        // ── Public API / 公開 API ─────────────────────────

        /// <summary>
        /// Assign the player transform at runtime / 執行時動態指定跟隨目標
        /// </summary>
        public void SetTarget(Transform t) => target = t;

        /// <summary>
        /// Snap camera instantly to target without smoothing / 瞬間移到目標位置，跳過平滑
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 pivot      = target.position + targetOffset;
            Quaternion rot     = Quaternion.Euler(_pitch, _yaw, 0f);
            transform.position = pivot + rot * Vector3.back * distance;
            transform.LookAt(pivot);
            _posVelocity = Vector3.zero;
        }
    }
}
