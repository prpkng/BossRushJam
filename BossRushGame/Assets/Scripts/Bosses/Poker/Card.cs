using System;
using BRJ.Player;
using BRJ.Systems;
using BRJ.Systems.Common;
using UnityEngine;

namespace BRJ.Bosses.Poker
{
    
    public class Card : MonoBehaviour
    {
        public enum Type
        {
            None = -1,
            SpadesAce = 0,
            HeartsAce,
            ClubsAce,
            DiamondsAce,
        }

        public const int CardCount = 4;
        
        public float cameraWeight = 1;
        public HealthBehavior health;
        public Transform spriteTransform;
        public SpriteRenderer frontSprite;
        public float moveRotationForce;
        public float spriteLerpSpeed = 10;
        
        private Vector3 movementVel;
        private Vector3 lastPos;
        private Type cardClass;
        
        public void SetClass(Type @class, Sprite classSprite)
        {
            frontSprite.sprite = classSprite;
            cardClass = @class;
        }

        public void Activate(PokerBoss boss)
        {
            print("Card Activated");
            switch (cardClass)
            {
                case Type.DiamondsAce:
                    gameObject.AddComponent<CardAttackDiamonds>();
                    break;
                case Type.SpadesAce:
                    gameObject.AddComponent<CardAttackSpades>();
                    break;
                case Type.HeartsAce:
                    boss.bossHealth.AddHealth(boss.heartsHealthRecover);
                    Destroy(gameObject);
                    break;
                case Type.ClubsAce:
                    gameObject.AddComponent<ClubsWallAttack>();
                    break;
                default:
                    break;
            }

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