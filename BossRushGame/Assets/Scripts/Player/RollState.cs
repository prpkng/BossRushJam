using System;
using Cysharp.Threading.Tasks;
using FSM;
using UnityEngine;

namespace Game.Player.States
{
    public class RollState : State
    {
        private PlayerManager playerManager;
        public Vector2 rollDirection;

        public override async void OnEnter()
        {
            if (!playerManager) playerManager = parent.GetComponent<PlayerManager>();

            rollDirection = InputManager.MoveVector.normalized;

            await UniTask.WaitForSeconds(playerManager.rollDuration);


            if (Mathf.Abs(playerManager.rb.linearVelocity.sqrMagnitude) < Mathf.Epsilon)
                parent.SetState(new IdleState());
            else
                parent.SetState(new MoveState());
        }

        public override void FixedTick(float delta)
        {
            playerManager.rb.linearVelocity = rollDirection * playerManager.rollSpeed;
        }
    }
}