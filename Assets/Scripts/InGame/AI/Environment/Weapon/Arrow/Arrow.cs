using FYP.InGame.AI.Agent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Weapon
{
    public class Arrow : MonoBehaviour
    {
        private GameObject sprite;

        private int damage;
        private float arrowSpeed;
        private float damageScaling;

        // facing right: 0, facing up: 1, facing left: 2, facing down: 3
        private int direction;
        private Point origin;

        private Transform parent;

        private Vector2 startPosition;
        private int cellMoved;
        private int CellMoved
        {
            get { return cellMoved; }
            set
            {
                cellMoved = value;
                onMovedToNewCell(cellMoved + 1);
            }
        }

        private GameObject user;

        private void Awake()
        {
            sprite = transform.GetChild(0).gameObject;
            parent = GameObject.FindGameObjectWithTag("GameObjectContainer").transform;
        }

        void Update()
        {
            if (direction == 0)
            {
                transform.position += new Vector3(arrowSpeed * Time.deltaTime, 0);
            }
            else if (direction == 1)
            {
                transform.position += new Vector3(0, arrowSpeed * Time.deltaTime);
            }
            else if (direction == 2)
            {
                transform.position += new Vector3(-arrowSpeed * Time.deltaTime, 0);
            }
            else if (direction == 3)
            {
                transform.position += new Vector3(0, -arrowSpeed * Time.deltaTime);
            }
            float distance = Vector2.Distance(startPosition, transform.position);
            if (distance > MapController.Instance.cellSize)
            {
                ++CellMoved;
                startPosition = transform.position;
            }
        }

        public void initialize(InGame.Weapon.ScriptableArrow sa, int facing, Point origin, float damageScaling, int sortingOrder, GameObject user)
        {
            damage = sa.damage;
            arrowSpeed = sa.speed;
            direction = facing;
            this.damageScaling = damageScaling;
            this.origin = origin;
            this.user = user;

            CellMoved = 0;

            sprite.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            transform.SetParent(parent);
            if (facing == 1)
            {
                --sprite.GetComponent<SpriteRenderer>().sortingOrder;
            }

            sprite.transform.position += new Vector3(0, 0.2f);

            startPosition = transform.position;
        }

        private void onMovedToNewCell(int adjustedCellMoved)
        {
            List<GameObject> objectsInRange = new List<GameObject>();
            Tile t;
            if (direction == 0)
            {
                if (origin.x + adjustedCellMoved >= MapController.Instance.playableMapSize.x)
                {
                    Destroy(gameObject); return;
                }
                t = MapController.Instance.tileMatrix[origin.y][origin.x + adjustedCellMoved];
            }
            else if (direction == 1)
            {
                if (origin.y - adjustedCellMoved < 0)
                {
                    Destroy(gameObject); return;
                }
                t = MapController.Instance.tileMatrix[origin.y - adjustedCellMoved][origin.x];
            }
            else if (direction == 2)
            {
                if (origin.x - adjustedCellMoved < 0)
                {
                    Destroy(gameObject); return;
                }
                t = MapController.Instance.tileMatrix[origin.y][origin.x - adjustedCellMoved];
            }
            else
            {
                if (origin.y + adjustedCellMoved >= MapController.Instance.playableMapSize.y)
                {
                    Destroy(gameObject); return;
                }
                t = MapController.Instance.tileMatrix[origin.y + adjustedCellMoved][origin.x];
            }

            if (t.tileState == Tile.TileStates.hasPlayer)
            {
                objectsInRange.AddRange(t.currentObjects.
                    Where(go => !GameManager.Instance.isSameTeam(user, go)));
            }
            else if (t.tileState == Tile.TileStates.hasBreakable)
            {
                objectsInRange.AddRange(t.currentObjects);
            }

            // AI Training
            float totalArraowDamage = 0;


            foreach (GameObject obj in objectsInRange)
            {
                obj.GetComponent<IDamageable>().takeDamage((int)(damage * damageScaling), DamageType.physicalHit);
                totalArraowDamage += (int)(damage * damageScaling);
            }

            // AI Training
            print($"add reward {totalArraowDamage}");
            user.GetComponent<DashingGameAgent>().AddReward(totalArraowDamage);

            if (objectsInRange.Count > 0) { Destroy(gameObject); }
        }
    }
}