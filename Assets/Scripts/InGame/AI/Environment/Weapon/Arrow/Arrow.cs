using FYP.InGame.AI.Agent;
using FYP.InGame.AI.Environment.Character;
using FYP.InGame.Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Weapon
{
    public class Arrow : MonoBehaviour
    {
        public MapController mapController;
        public AIManager aiManager;
        public GameManager gameManager;
        public MapObjectManager mapObjectManager;

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
            if (distance > mapController.cellSize)
            {
                ++CellMoved;
                startPosition = transform.position;
            }
        }

        public void initialize(InGame.Weapon.ScriptableArrow sa, int facing, Point origin, float damageScaling, int sortingOrder, GameObject user)
        {
            mapController = user.GetComponent<CharacterBuilder>().mapController;
            aiManager = user.GetComponent<CharacterBuilder>().aiManager;
            gameManager = user.GetComponent<CharacterBuilder>().gameManager;
            mapObjectManager = user.GetComponent<CharacterBuilder>().mapObjectManager;

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
                if (origin.x + adjustedCellMoved >= mapController.playableMapSize.x)
                {
                    Destroy(gameObject); return;
                }
                t = mapController.tileMatrix[origin.y][origin.x + adjustedCellMoved];
            }
            else if (direction == 1)
            {
                if (origin.y - adjustedCellMoved < 0)
                {
                    Destroy(gameObject); return;
                }
                t = mapController.tileMatrix[origin.y - adjustedCellMoved][origin.x];
            }
            else if (direction == 2)
            {
                if (origin.x - adjustedCellMoved < 0)
                {
                    Destroy(gameObject); return;
                }
                t = mapController.tileMatrix[origin.y][origin.x - adjustedCellMoved];
            }
            else
            {
                if (origin.y + adjustedCellMoved >= mapController.playableMapSize.y)
                {
                    Destroy(gameObject); return;
                }
                t = mapController.tileMatrix[origin.y + adjustedCellMoved][origin.x];
            }

            if (t.tileState == Tile.TileStates.hasPlayer)
            {
                objectsInRange.AddRange(t.currentObjects.
                    Where(go => !gameManager.isSameTeam(user, go)));
            }
            else if (t.tileState == Tile.TileStates.hasBreakable)
            {
                objectsInRange.AddRange(t.currentObjects);
            }

            foreach (GameObject obj in objectsInRange)
            {
                obj.GetComponent<IDamageable>().takeDamage((int)(damage * damageScaling), DamageType.physicalHit);
            }

            if (objectsInRange.Count != 0)
            {
                // AI Training
                print($"add reward {aiManager.reward}");
                user.GetComponent<DashingGameAgent>().AddReward(aiManager.reward);
            }
            else {
                // AI Training
                // print($"add reward {-aiManager.microReward}");
                user.GetComponent<DashingGameAgent>().AddReward(-aiManager.microReward);
            }

            if (objectsInRange.Count > 0) { Destroy(gameObject); }
        }
    }
}