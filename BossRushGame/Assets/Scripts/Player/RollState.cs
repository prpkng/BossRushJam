using PrimeTween;
using UnityEngine;
using UnityHFSM;

namespace Game.Player
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

        public override void OnEnter()
        {
            timer.Reset();
            rollDirection = InputManager.MoveVector.normalized;
            
            playerManager.activeGun.gameObject.SetActive(false);
            playerManager.playerSprite.transform.localScale = new Vector2(rollDirection.x < 0 ? -1 : 1, 1);
            
            playerManager.playerHitbox.SetInvulnerable(playerManager.rollInvulnerabilityDuration);
        }

        public override void OnLogic()
        {
            playerManager.Rb.linearVelocity = rollDirection * playerManager.rollSpeed;

            if (timer.Elapsed < playerManager.rollDuration) return;
            fsm.StateCanExit();
        }

        public override void OnExit()
        {
            playerManager.activeGun.gameObject.SetActive(true);
            Tween.Delay(playerManager.rollCooldown, () => playerManager.CanRoll = true);
        }
    }
}