using System;
using FSM;
using UnityEngine;

namespace Game.Player.States
{
    public class MoveState : State
    {
        private PlayerManager playerManager;

        public override void OnEnter()
        {
            if (!playerManager) playerManager = parent.GetComponent<PlayerManager>();
        }

        public override void FixedTick(float delta)
        {
            var moveInput = InputManager.MoveVector;
            var targetSpeed = moveInput * playerManager.movementSpeed;
            var speedDiff = targetSpeed - playerManager.rb.linearVelocity;
            var accelRate = Mathf.Abs(moveInput.sqrMagnitude) < Mathf.Epsilon
                                ? playerManager.deceleration
                                : playerManager.acceleration;

            playerManager.rb.linearVelocity += speedDiff * accelRate;

            if (Mathf.Abs(playerManager.rb.linearVelocity.sqrMagnitude) < Mathf.Epsilon)
                parent.SetState(new IdleState());
        }
    }
}