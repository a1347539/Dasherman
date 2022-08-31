using FYP.Global;
using FYP.InGame.Map;
using FYP.InGame.Photon;
using FYP.InGame.Weapon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterAttackController : MonoBehaviourPun
    {
        [SerializeField]
        private ScriptableArrow arrowData;

        private CharacterController controller;
        private CharacterBuilder builder;

        public WeaponAttackType attackType { get; private set; }

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
            get { print(baseWeaponDamage * physicalDamageScaling); return(int)(baseWeaponDamage * physicalDamageScaling); }
        }

        public GameObject weapon { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            builder = GetComponent<CharacterBuilder>();
            if (!photonView.IsMine) return;
            controller.onPointChanged += handleAttackEnemiesInPath;
            controller.onAttack += handleAttack;
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            controller.onPointChanged -= handleAttackEnemiesInPath;
            controller.onAttack -= handleAttack;
        }

        public void initializeStats(int baseDashingDamage, float physicalDamageScaling, float magicDamageScaling, GameObject weapon, WeaponAttackType attackType) {
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
            List<PhotonView> getEnemiesInPath(Point oldPoint, Point newPoint)
            {
                
                List<PhotonView> playersInPath = new List<PhotonView>();
                if (oldPoint.x == newPoint.x && oldPoint.y == newPoint.y) return playersInPath;

                if (oldPoint.x == newPoint.x)
                {
                    if (oldPoint.y < newPoint.y)
                    {
                        for (int i = oldPoint.y + 1; i <= newPoint.y; ++i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[i][newPoint.x].currentObjects.Where(pv => !PlayerManager.isSameTeam(photonView, pv)));
                        }
                    }
                    else
                    {
                        // oldPoint.y > newPoint.y
                        for (int i = oldPoint.y - 1; i >= newPoint.y; --i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[i][newPoint.x].currentObjects.Where(pv => !PlayerManager.isSameTeam(photonView, pv)));
                        }
                    }
                }
                else if (oldPoint.y == newPoint.y)
                {
                    if (oldPoint.x < newPoint.x)
                    {
                        for (int i = oldPoint.x + 1; i <= newPoint.x; ++i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[newPoint.y][i].currentObjects.Where(pv => !PlayerManager.isSameTeam(photonView, pv)));
                        }
                    }
                    else
                    {
                        for (int i = oldPoint.x - 1; i >= newPoint.x; --i)
                        {
                            playersInPath.AddRange(MapController.Instance.tileMatrix[newPoint.y][i].currentObjects.Where(pv => !PlayerManager.isSameTeam(photonView, pv)));
                        }
                    }
                }
                foreach (var pv in playersInPath)
                {
                    print($"{pv.Owner.NickName} is in the path");
                }
                return playersInPath;
            }

            List<PhotonView> enemiesInPath = getEnemiesInPath(oldPoint, newPoint);
            foreach (PhotonView enemy in enemiesInPath)
            {
                // if (enemy.Owner == PhotonNetwork.LocalPlayer) continue;
                // print($"{enemy.Owner.NickName} take damage");
                enemy.GetComponent<IDamageable>().takeDamage(DashingDamage, DamageType.dashing, photonView.IsRoomView ? null : photonView);
            }
        }

        private void handleAttack(int facingIndex) {
            if (attackType == WeaponAttackType.Ranged)
            {
                rangedAttack(facingIndex);
            }
            else {
                handleAttackObjectsInRange(facingIndex);
            }
        }

        private void rangedAttack(int facingIndex) {
            photonView.RPC("createArrowRPC", RpcTarget.All, facingIndex);
        }

        [PunRPC]
        private void createArrowRPC(int facingIndex) {
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

            arrow.GetComponent<Arrow>().initialize(arrowData, facingIndex, controller.currentPoint, physicalDamageScaling, builder.Sprite.GetComponent<SortingGroup>().sortingOrder, photonView);
        }


        private void handleAttackObjectsInRange(int facingIndex)
        {
            List<PhotonView> objectsInRange = new List<PhotonView>();
            for (int i = controller.currentPoint.x - 3, p = 0; p < ScriptableWeapon.SIZE; ++i, ++p) {
                if (i < 0 || i >= MapController.Instance.playableMapSize.x) { continue; }
                for (int j = controller.currentPoint.y - 3, q = 0; q < ScriptableWeapon.SIZE; ++j, ++q) {
                    if (j < 0 || j >= MapController.Instance.playableMapSize.y) { continue; }
                    if (weapon.GetComponent<WeaponController>().ranges[facingIndex][q].column[p])
                    {
                        if (MapController.Instance.tileMatrix[j][i].tileState == Tile.TileStates.hasPlayer)
                        {
                            objectsInRange.AddRange(MapController.Instance.tileMatrix[j][i].currentObjects.Where(pv => !PlayerManager.isSameTeam(photonView, pv)));
                        }
                        else if (MapController.Instance.tileMatrix[j][i].tileState == Tile.TileStates.hasBreakable) {
                            objectsInRange.AddRange(MapController.Instance.tileMatrix[j][i].currentObjects);
                        }
                    }
                }
            }
            foreach (PhotonView obj in objectsInRange)
            {
                obj.GetComponent<IDamageable>().takeDamage(WeaponDamage, DamageType.physicalHit, photonView.IsRoomView ? null : photonView);
            }
        }
    }
}