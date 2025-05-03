using System.Collections;
using System.Collections.Generic;
using TopDown;
using UnityEngine;

namespace TopDown_Template
{
    public class BulletSpawner : MonoBehaviour
    {
        #region Variable
        [SerializeField] private GameObject _visualProjectTile;
        [SerializeField] private float _sizeProjecTile = 1;
        [SerializeField] private LayerMask _hitLayer;
        #endregion

        #region BulletSpawner Method
        public void SpawnBullet(Vector2 positionSpawn, Quaternion direction,
            int damage, float force, float speed, float lifeTime)
        {
            GameObject bullet = PoolManager.Instance.Spawn(
                Settings.Pool_Bullet,
                positionSpawn,
                direction
            );

            var b_projectile = bullet.GetComponent<Projectile>();
            b_projectile.SetProjecTile(
                damage, 
                force, 
                speed, 
                lifeTime, 
                _sizeProjecTile, 
                _hitLayer, 
                _visualProjectTile
            );

            // Projectile t_projectTile = PoolingManager.Instance.GetObjectInPool();
            // t_projectTile.transform.rotation = direction;
            // t_projectTile.transform.position = positionSpawn;
            // t_projectTile.SetProjecTile(damage, force, speed, lifeTime, _sizeProjecTile, _hitLayer, _visualProjectTile);
        }
        #endregion
    }
}

