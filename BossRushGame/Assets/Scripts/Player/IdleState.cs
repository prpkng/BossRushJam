using System;
using FSM;
using UnityEngine;

namespace Game.Player.States
{
    public class IdleState : State
    {
        public override void OnEnter()
        {
        }

        public override void FixedTick(float delta)
        {
            var moveInput = InputManager.moveVector;
            if (moveInput.sqrMagnitude > 0)
                parent.SetState(new MoveState());
        }
    }
}