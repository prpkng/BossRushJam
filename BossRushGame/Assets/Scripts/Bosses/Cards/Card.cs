using System;
using Game.Player;
using UnityEngine;

namespace Game.Bosses.Cards
{
    
    public class Card : MonoBehaviour
    {
        public enum Type
        {
            SpadesAce,
            HeartsAce,
            ClubsAce,
            DiamondsAce,
        }
        
        public float cameraWeight = 1;
        public Transform spriteTransform;
        public SpriteRenderer frontSprite;
        public float moveRotationForce;
        public float spriteLerpSpeed = 10;
        private Vector3 lastPos;
        private Type cardClass;
        
        public void SetClass(Type @class, Sprite classSprite)
        {
            frontSprite.sprite = classSprite;
            cardClass = @class;
        }
        
        private void Update()
        {
            var vel = transform.position - lastPos;
            lastPos = transform.position;
            
            spriteTransform.localEulerAngles = Mathf.LerpAngle(
                spriteTransform.localEulerAngles.z, 
                vel.x * moveRotationForce,
                Time.deltaTime * spriteLerpSpeed) * Vector3.forward;
        }

        private void OnEnable()
        {
            CameraManager.Instance.AddTarget(transform, cameraWeight);
        }

        private void OnDisable()
        {
            CameraManager.Instance.RemoveTarget(transform);
        }
    }
}