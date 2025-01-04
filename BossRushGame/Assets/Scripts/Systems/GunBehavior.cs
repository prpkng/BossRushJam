using UnityEngine;

namespace Game.Systems
{
    public class GunBehavior : MonoBehaviour
    {
        [SerializeField] private Transform muzzleTransform;

        public void FireBullet(GameObject bulletPrefab, float bulletForce, float forceMultiplier = 1)
        {
            var bullet = Instantiate(bulletPrefab, muzzleTransform.position, transform.rotation);

            bullet.GetComponent<Rigidbody2D>().linearVelocity =
                bulletForce * forceMultiplier * bullet.transform.right;
        }
    }
}
