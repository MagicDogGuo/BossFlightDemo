using UnityEngine;

namespace BossFlightDemo.Player.States
{
    public class PlayerIdleState : PlayerBaseState
    {
        public override void Enter(PlayerController owner)
        {
            owner.Animator?.SetBool("IsMoving", false);
        }

        public override void Execute(PlayerController owner)
        {
            HandleMovement(owner);

            bool isMoving = owner.MoveInput.sqrMagnitude > 0.01f;
            owner.Animator?.SetBool("IsMoving", isMoving);

            if (owner.AttackPressed)
            {
                owner.StateMachine.ChangeState(owner.AttackState);
                return;
            }
            if (owner.DodgePressed)
            {
                owner.StateMachine.ChangeState(owner.DodgeState);
                return;
            }
            if (owner.ParryPressed)
            {
                owner.StateMachine.ChangeState(owner.ParryState);
            }
        }

        public override void Exit(PlayerController owner)
        {
            owner.Animator?.SetBool("IsMoving", false);
        }
    }
}
