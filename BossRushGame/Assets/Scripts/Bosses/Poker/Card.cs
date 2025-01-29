using System;
using BRJ.Player;
using BRJ.Systems;
using BRJ.Systems.Common;
using BRJ.Systems.Visual;
using PrimeTween;
using UnityEngine;

namespace BRJ.Bosses.Poker
{

    public class Card : MonoBehaviour
    {
        public enum Suits
        {
            None = -1,
            Spades = 0,
            Hearts,
            Clubs,
            Diamonds,
        }

        public const int CardCount = 4;

        public float cameraWeight = 1;
        public HealthBehavior health;
        public Transform spriteTransform;
        public GameObject exclamationMark;
        public SpriteRenderer frontSprite;
        public new Collider2D collider;
        public SineLocalPosition cardSine;
        public float moveRotationForce;
        public float spriteLerpSpeed = 10;

        public ShakeSettings hitShakeSettings;

        private Vector3 movementVel;
        private Vector3 lastPos;
        private Suits cardSuit;

        public void SetClass(Suits suit, Sprite classSprite)
        {
            frontSprite.sprite = classSprite;
            cardSuit = suit;
        }

        public void BallHit()
        {
            Tween.ShakeLocalPosition(spriteTransform, hitShakeSettings);
        }

        public void Activate(PokerBoss boss)
        {
            print("Card Activated");
            switch (cardSuit)
            {
                case Suits.Diamonds:
                    {
                        var attack = gameObject.AddComponent<CardAttackDiamonds>();
                        boss.attackManager.AddCard(attack);
                        break;
                    }
                case Suits.Spades:
                    {
                        var attack = gameObject.AddComponent<CardAttackSpades>();
                        boss.attackManager.AddCard(attack);
                        break;
                    }
                case Suits.Hearts:
                    {
                        boss.bossHealth.AddHealth(boss.heartsHealthRecover);
                        Destroy(gameObject);
                        break;
                    }
                case Suits.Clubs:
                    {
                        var attack = gameObject.AddComponent<ClubsWallAttack>();
                        boss.attackManager.AddCard(attack);
                        break;
                    }
                default:
                    break;
            }

            cardSine.enabled = true;
            health.enabled = true;
        }

        private void FixedUpdate()
        {
            movementVel = transform.position - lastPos;
            lastPos = transform.position;
        }

        private void Update()
        {

            spriteTransform.localEulerAngles = Mathf.LerpAngle(
                spriteTransform.localEulerAngles.z,
                movementVel.x * moveRotationForce,
                Time.deltaTime * spriteLerpSpeed) * Vector3.forward;
        }

        private void OnEnable()
        {
            Game.Instance.Camera.AddTarget(transform, cameraWeight);
        }

        private void OnDisable()
        {
            Game.Instance.Camera.RemoveTarget(transform);
        }
    }
}