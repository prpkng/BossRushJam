using UnityEngine;

namespace Game.Systems.Common
{
    public class GunBehavior : MonoBehaviour
    {
        [SerializeField] private Transform muzzleTransform;

        public void FireBullet(GameObject bulletPrefab, Vector2 direction, float bulletForce, float forceMultiplier = 1)
        {
            var bullet = Instantiate(bulletPrefab, muzzleTransform.position, transform.rotation);

            var rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = bulletForce * forceMultiplier * direction;
            bullet.transform.right = rb.linearVelocity.normalized;
        }
    }
}
