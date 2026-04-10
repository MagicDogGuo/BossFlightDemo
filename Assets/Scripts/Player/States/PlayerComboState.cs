using UnityEngine;

namespace BossFlightDemo.Player.States
{
    /// <summary>
    /// Handles one hit in the three-hit combo chain.
    /// comboIndex 0 = Attack1 (1.0×), 1 = Attack2 (1.3×), 2 = Attack3 (2.0×).
    ///
    /// Timeline per hit:
    ///   [0 ────── windup ──── activeDuration ──── timingWindow ─────]
    ///    ↑                    ↑                   ↑
    ///    Enter()          hitbox ON → OFF      buffer input here → chain or Idle
    ///
    /// Interview note: input buffer starts from Enter(), not just the timingWindow,
    /// so an early press still chains — preventing the "dead-input" feel.
    /// </summary>
    public class PlayerComboState : PlayerBaseState
    {
        private readonly int _comboIndex;

        private float _timer;
        private bool  _hitboxEnabled;
        private bool  _inputBuffered;
        private bool  _chainingToNext;   // Suppresses BackToIdle trigger when chaining

        public PlayerComboState(int comboIndex)
        {
            _comboIndex = comboIndex;
        }

        public override void Enter(PlayerController owner)
        {
            _timer          = 0f;
            _hitboxEnabled  = false;
            _inputBuffered  = false;
            _chainingToNext = false;

            // Animator.md: SetInteger BEFORE SetTrigger so the transition condition is
            // already correct when the Animator evaluates it this frame.
            owner.Animator?.SetInteger("ComboIndex", _comboIndex);
            owner.Animator?.SetTrigger("Attack");
        }

        public override void Execute(PlayerController owner)
        {
            _timer += Time.deltaTime;

            var data = owner.GetComboData(_comboIndex);
            if (data == null)
            {
                owner.StateMachine.ChangeState(owner.IdleState);
                return;
            }

            // Record attack input at any point — this is the input buffer.
            // Even a press during wind-up will chain into the next hit.
            if (owner.AttackPressed)
                _inputBuffered = true;

            // ── Hitbox lifecycle ─────────────────────────────
            float hitboxOpenAt  = data.windup;
            float hitboxCloseAt = data.windup + data.activeDuration;

            if (!_hitboxEnabled && _timer >= hitboxOpenAt)
            {
                _hitboxEnabled = true;
                int dmg = Mathf.RoundToInt(owner.AttackDamage * data.damageMultiplier);
                owner.AttackHitbox?.EnableHitbox(dmg);
            }

            if (_hitboxEnabled && _timer >= hitboxCloseAt)
            {
                _hitboxEnabled = false;
                owner.AttackHitbox?.DisableHitbox();
            }

            // ── End of timing window: chain or return to Idle ─
            float stateEnd = data.windup + data.activeDuration + data.timingWindow;
            if (_timer >= stateEnd)
            {
                bool isLastHit = _comboIndex >= 2;

                if (_inputBuffered && !isLastHit)
                {
                    _chainingToNext = true;
                    owner.StateMachine.ChangeState(owner.GetComboState(_comboIndex + 1));
                }
                else
                {
                    owner.StateMachine.ChangeState(owner.IdleState);
                }
            }
        }

        public override void Exit(PlayerController owner)
        {
            if (_hitboxEnabled)
            {
                owner.AttackHitbox?.DisableHitbox();
                _hitboxEnabled = false;
            }

            // Only trigger BackToIdle when returning to Idle, not when chaining.
            // The next ComboState.Enter() will immediately set ComboIndex + Attack.
            if (!_chainingToNext)
                owner.Animator?.SetTrigger("BackToIdle");

            _timer          = 0f;
            _chainingToNext = false;
        }
    }
}
