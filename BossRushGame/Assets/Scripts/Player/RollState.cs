using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;
using PrimeTween;
using UnityHFSM;

namespace Game.Player.States
{
    public class RollState : StateBase
    {
        private readonly PlayerManager playerManager;
        private readonly Timer timer;
        private Vector2 rollDirection;


        public RollState(PlayerManager player) : base(needsExitTime: true, isGhostState: false)
        {
            playerManager = player;
            timer = new Timer();
        }

        private Tween rollTween;
        public override void OnEnter()
        {
            timer.Reset();
            rollDirection = InputManager.MoveVector.normalized;

            var flipValue = rollDirection.x > 0 || rollDirection.y > 0 ? -1 : 1;

            rollTween.Stop();
            playerManager.playerSprite.transform.rotation = Quaternion.identity;

            rollTween = Tween.Custom(
                0f,
                360f * flipValue,
                playerManager.rollDuration,
                f => playerManager.playerSprite.transform.eulerAngles = Vector3.forward * f
            );
            
            playerManager.playerHitbox.SetInvulnerable(playerManager.rollInvulnerabilityDuration);
        }

        public override void OnLogic()
        {
            playerManager.rb.linearVelocity = rollDirection * playerManager.rollSpeed;

            if (timer.Elapsed < playerManager.rollDuration) return;
            fsm.StateCanExit();
        }

        public override void OnExit()
        {
            Tween.Delay(playerManager.rollCooldown, () => playerManager.canRoll = true);
        }
    }
}