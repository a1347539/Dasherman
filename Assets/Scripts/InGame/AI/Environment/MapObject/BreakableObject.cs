using FYP.Global.InGame;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

namespace FYP.InGame.AI.Environment
{
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        public MapController mapController;
        public AIManager aiManager;
        public GameManager gameManager;
        public MapObjectManager mapObjectManager;

        public int currentHealth { get; private set; }

        public int debugHealth;

        public Point currentPoint { get; private set; }

        public int maxHealth { get; private set; }

        public void setCurrentPoint(Point p)
        {
            onChangeCurrenPoint(currentPoint, p);
        }

        private void Awake()
        {
            mapController = transform.parent.parent.GetComponent<Containers>().mapController;
            aiManager = transform.parent.parent.GetComponent<Containers>().aiManager;
            gameManager = transform.parent.parent.GetComponent<Containers>().gameManager;
            mapObjectManager = transform.parent.parent.GetComponent<Containers>().mapObjectManager;
        }

        public void initialize(int health, Point point)
        {
            Vector2 cellCenterPosition = mapController.pointToTile(point).worldPositionOfCellCenter;
            transform.localPosition = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + mapController.objectSpriteOffsetInY
                );

            transform.localScale = mapController.objectSpriteScaling;

            maxHealth = health;
            debugHealth = health;
            currentHealth = health;
            currentPoint = point;
            GetComponent<Renderer>().sortingOrder = point.y;
            mapController.tileMatrix[point.y][point.x].objectEnter(gameObject);
        }

        void IDamageable.takeDamage(int damage, DamageType damageType, GameObject pv)
        {
            int rawHealth = currentHealth - damage;
            if (rawHealth <= 0)
            {
                mapController.tileMatrix[currentPoint.y][currentPoint.x].objectExit(gameObject);
                transform.position = BreakableObjectKeys.DeadObjectPosition;
                return;
            }
            currentHealth -= damage;
            debugHealth -= damage;
        }

        private void onChangeCurrenPoint(Point oldPoint, Point newPoint)
        {
            // print(newPoint.x + " " + newPoint.y);

            Vector2 cellCenterPosition = mapController.pointToTile(newPoint).worldPositionOfCellCenter;
            transform.localPosition = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + mapController.objectSpriteOffsetInY
                );

            mapController.tileMatrix[oldPoint.y][oldPoint.x].objectExit(gameObject);
            mapController.tileMatrix[newPoint.y][newPoint.x].objectEnter(gameObject);
            currentPoint = newPoint;
            GetComponent<Renderer>().sortingOrder = newPoint.y;
        }
    }
}