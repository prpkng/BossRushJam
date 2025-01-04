using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Collections;
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


        public override void OnEnter()
        {
            timer.Reset();
            rollDirection = InputManager.MoveVector.normalized;

            int flipValue = rollDirection.x > 0 || rollDirection.y > 0 ? -1 : 1;

            playerManager.playerSprite.transform.DOKill(true);
            playerManager.playerSprite.transform.DORotate(
                360f * flipValue * Vector3.forward,
                playerManager.rollDuration,
                RotateMode.WorldAxisAdd
            );
        }

        public override void OnLogic()
        {
            playerManager.rb.linearVelocity = rollDirection * playerManager.rollSpeed;

            if (timer.Elapsed < playerManager.rollDuration) return;
            fsm.StateCanExit();
        }

        public override async void OnExit()
        {
            await UniTask.WaitForSeconds(playerManager.rollCooldown);
            playerManager.canRoll = true;
        }
    }
    // public class RollState : State
    // {
    //     private PlayerManager playerManager;
    //     public Vector2 rollDirection;

    //     public override async void OnEnter()
    //     {
    //         if (!playerManager) playerManager = parent.GetComponent<PlayerManager>();

    //         rollDirection = InputManager.MoveVector.normalized;

    //         int flipValue = rollDirection.x > 0 || rollDirection.y > 0 ? -1 : 1;

    //         playerManager.playerSprite.transform.DOKill(true);
    //         playerManager.playerSprite.transform.DORotate(
    //             360f * flipValue * Vector3.forward,
    //             playerManager.rollDuration,
    //             RotateMode.WorldAxisAdd
    //         );

    //         await UniTask.WaitForSeconds(playerManager.rollDuration);

    //         if (playerManager.rb.linearVelocity.sqrMagnitude < Mathf.Epsilon)
    //             parent.SetState(new IdleState());
    //         else
    //             parent.SetState(new MoveState());


    //         await UniTask.WaitForSeconds(playerManager.rollCooldown);

    //         playerManager.canRoll = true;
    //     }

    //     public override void FixedTick(float delta)
    //     {
    //         playerManager.rb.linearVelocity = rollDirection * playerManager.rollSpeed;
    //     }
    // }
}