using Game.Player;
using Game.Systems;
using PrimeTween;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class BallDeathParticles : MonoBehaviour
    {
        public SnookerBall ball;
        public SpriteRenderer[] sprites;

        public float duration;
        public float displacement;
        public Ease ease = Ease.OutCubic;
        public void Play()
        {
            CameraManager.Instance.ShakeCamera(CameraManager.Instance.defaultWeakShake);
            gameObject.SetActive(true);
            transform.parent = null;
            foreach (var spriteRenderer in sprites)
            {
                spriteRenderer.material.SetFloat("_Shift", ball.CurrentHue);
            }

            for (int i = 0; i < sprites.Length; i++)
            {
                var spriteTransform = sprites[i].transform;
                var dir = (spriteTransform.position - transform.position).normalized;
                Tween.Position(
                    spriteTransform,
                    spriteTransform.position + dir * displacement,
                    duration,
                    ease
                );
                Tween.Alpha(
                    sprites[i],
                    0,
                    duration,
                    ease
                );
            }
            
        }
    }
}