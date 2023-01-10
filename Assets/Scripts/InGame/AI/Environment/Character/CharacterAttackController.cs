using FYP.InGame.AI.Environment.Weapon;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterAttackController : MonoBehaviour
    {
        [SerializeField]
        private InGame.Weapon.ScriptableArrow arrowData;

        private CharacterController controller;
        private CharacterBuilder builder;

        public InGame.Weapon.WeaponAttackType attackType { get; private set; }

        public float physicalDamageScaling = 1;
        public float magicDamageScaling = 1;
        private int baseDashingDamage;
        public int DashingDamage
        {
            get { return (int)(baseDashingDamage * physicalDamageScaling); }
        }

        private int baseWeaponDamage;
        public int WeaponDamage
        {
            get { print(baseWeaponDamage * physicalDamageScaling); return (int)(baseWeaponDamage * physicalDamageScaling); }
        }

        public GameObject weapon { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            builder = GetComponent<CharacterBuilder>();
            controller.onPointChanged += handleAttackEnemiesInPath;
            controller.onAttack += handleAttack;
        }

        private void OnDestroy()
        {
            controller.onPointChanged -= handleAttackEnemiesInPath;
            controller.onAttack -= handleAttack;
        }

        public void initializeStats(int baseDashingDamage, float physicalDamageScaling, float magicDamageScaling, GameObject weapon, InGame.Weapon.WeaponAttackType attackType)
        {
            this.physicalDamageScaling = physicalDamageScaling;
            this.magicDamageScaling = magicDamageScaling;
            this.baseDashingDamage = baseDashingDamage;
            this.weapon = weapon;
            this.attackType = attackType;
            controller.AttackSpeed = weapon.GetComponent<WeaponController>().attackSpeed;
            baseWeaponDamage = weapon.GetComponent<WeaponController>().damage;
        }

        private void handleAttackEnemiesInPath(Point oldPoint, Point newPoint)
        {
            List<GameObject> getEnemiesInPath(Point oldPoint, Point newPoint)
            {

                List<GameObject> playersInPath = new List<GameObject>();
                if (oldPoint.x == newPoint.x && oldPoint.y == newPoint.y) return playersInPath;

                if (oldPoint.x == newPoint.x)
                {
                    if (oldPoint.y < newPoint.y)
                    {
                        for (int i = oldPoint.y + 1; i <= newPoint.y; ++i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[i][newPoint.x].currentObjects.Where(ob => !GameManager.Instance.isSameTeam(gameObject, ob)));
                        }
                    }
                    else
                    {
                        // oldPoint.y > newPoint.y
                        for (int i = oldPoint.y - 1; i >= newPoint.y; --i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[i][newPoint.x].currentObjects.Where(ob => !GameManager.Instance.isSameTeam(gameObject, ob)));
                        }
                    }
                }
                else if (oldPoint.y == newPoint.y)
                {
                    if (oldPoint.x < newPoint.x)
                    {
                        for (int i = oldPoint.x + 1; i <= newPoint.x; ++i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[newPoint.y][i].currentObjects.Where(ob => !GameManager.Instance.isSameTeam(gameObject, ob)));
                        }
                    }
                    else
                    {
                        for (int i = oldPoint.x - 1; i >= newPoint.x; --i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[newPoint.y][i].currentObjects.Where(ob => !GameManager.Instance.isSameTeam(gameObject, ob)));
                        }
                    }
                }
                foreach (GameObject ob in playersInPath)
                {
                    print($"{ob.name} is in the path");
                }
                return playersInPath;
            }

            List<GameObject> enemiesInPath = getEnemiesInPath(oldPoint, newPoint);
            foreach (GameObject enemy in enemiesInPath)
            {
                // if (enemy.Owner == PhotonNetwork.LocalPlayer) continue;
                // print($"{enemy.Owner.NickName} take damage");
                enemy.GetComponent<IDamageable>().takeDamage(DashingDamage, DamageType.dashing);
            }
        }

        private void handleAttack(int facingIndex)
        {
            if (attackType == InGame.Weapon.WeaponAttackType.Ranged)
            {
                rangedAttack(facingIndex);
            }
            else
            {
                handleAttackObjectsInRange(facingIndex);
            }
        }

        private void rangedAttack(int facingIndex)
        {
            Quaternion q = Quaternion.identity;
            if (facingIndex == 1)
            {
                q = Quaternion.Euler(0, 0, 90);
            }
            else if (facingIndex == 2)
            {
                q = Quaternion.Euler(0, 0, 180);
            }
            else if (facingIndex == 3)
            {
                q = Quaternion.Euler(0, 0, 270);
            }
            GameObject arrow = Instantiate(arrowData.arrowPrefab, transform.position, q);

            arrow.GetComponent<Arrow>().initialize(arrowData, facingIndex, controller.currentPoint, physicalDamageScaling, builder.Sprite.GetComponent<SortingGroup>().sortingOrder, gameObject);
        }

        private void handleAttackObjectsInRange(int facingIndex)
        {
            List<GameObject> objectsInRange = new List<GameObject>();
            for (int i = controller.currentPoint.x - 3, p = 0; p < InGame.Weapon.ScriptableWeapon.SIZE; ++i, ++p)
            {
                if (i < 0 || i >= MapController.Instance.playableMapSize.x) { continue; }
                for (int j = controller.currentPoint.y - 3, q = 0; q < InGame.Weapon.ScriptableWeapon.SIZE; ++j, ++q)
                {
                    if (j < 0 || j >= MapController.Instance.playableMapSize.y) { continue; }
                    if (weapon.GetComponent<WeaponController>().ranges[facingIndex][q].column[p])
                    {
                        if (MapController.Instance.tileMatrix[j][i].tileState == Tile.TileStates.hasPlayer)
                        {
                            objectsInRange.AddRange(MapController.Instance.tileMatrix[j][i].currentObjects.Where(ob => !GameManager.Instance.isSameTeam(gameObject, ob)));
                        }
                        else if (MapController.Instance.tileMatrix[j][i].tileState == Tile.TileStates.hasBreakable)
                        {
                            objectsInRange.AddRange(MapController.Instance.tileMatrix[j][i].currentObjects);
                        }
                    }
                }
            }
            foreach (GameObject obj in objectsInRange)
            {
                obj.GetComponent<IDamageable>().takeDamage(WeaponDamage, DamageType.physicalHit);
            }
        }
    }
}