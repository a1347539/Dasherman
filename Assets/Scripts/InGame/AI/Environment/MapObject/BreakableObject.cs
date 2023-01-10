using FYP.Global.InGame;
using Photon.Pun;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        public int currentHealth { get; private set; }

        public int debugHealth;

        public Point currentPoint { get; private set; }

        public int maxHealth { get; private set; }

        public void initialize(int health, Point point)
        {
            Vector2 cellCenterPosition = MapController.Instance.pointToTile(point).worldPositionOfCellCenter;
            transform.position = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + MapController.Instance.objectSpriteOffsetInY
                );

            transform.localScale = MapController.Instance.objectSpriteScaling;

            transform.SetParent(MapObjectManager.Instance.breakableObjectContainer);
            maxHealth = health;
            debugHealth = health;
            currentHealth = health;
            currentPoint = point;
            GetComponent<Renderer>().sortingOrder = point.y;
            MapController.Instance.tileMatrix[point.y][point.x].objectEnter(gameObject);
        }

        void IDamageable.takeDamage(int damage, DamageType damageType, PhotonView pv)
        {
            int rawHealth = currentHealth - damage;
            if (rawHealth <= 0)
            {
                MapController.Instance.tileMatrix[currentPoint.y][currentPoint.x].objectExit(gameObject);
                transform.position = BreakableObjectKeys.DeadObjectPosition;
                return;
            }
            currentHealth -= damage;
            debugHealth -= damage;
        }
    }
}