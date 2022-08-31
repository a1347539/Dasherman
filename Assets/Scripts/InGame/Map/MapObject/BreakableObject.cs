using ExitGames.Client.Photon;
using FYP.Global.InGame;
using FYP.InGame.Map;
using FYP.InGame.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.BreakableObject
{
    public class BreakableObject : MonoBehaviourPun, IDamageable
    {
        public int currentHealth { get; private set; }

        public int debugHealth;

        public Point currentPoint { get; private set; }

        public int maxHealth { get; private set; }

        public void initialize(int health, Point point) {
            Vector2 cellCenterPosition = MapController.Instance.pointToTile(point).worldPositionOfCellCenter;
            transform.position = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + MapController.Instance.objectSpriteOffsetInY
                );

            transform.localScale = MapController.Instance.objectSpriteScaling;

            photonView.RPC("initializeRPC", RpcTarget.AllBuffered, health, point);
        }

        void IDamageable.takeDamage(int damage, DamageType damageType, PhotonView pv)
        {
            int rawHealth = currentHealth - damage;
            if (rawHealth <= 0) {
                photonView.RPC("objectDiesRPC", RpcTarget.All);
                return;
            }
            photonView.RPC("takeDamageRPC", RpcTarget.All, damage);
        }

        [PunRPC]
        public void initializeRPC(int health, Point p) {
            transform.SetParent(MapObjectManager.Instance.breakableObjectContainer);
            maxHealth = health;
            debugHealth = health;
            currentHealth = health;
            currentPoint = p;
            GetComponent<Renderer>().sortingOrder = p.y;
            MapController.Instance.tileMatrix[p.y][p.x].objectEnter(photonView);
        }

        [PunRPC]
        private void takeDamageRPC(int damage) {
            currentHealth -= damage;
            debugHealth -= damage;
        }

        [PunRPC]
        private void objectDiesRPC() {
            MapController.Instance.tileMatrix[currentPoint.y][currentPoint.x].objectExit(photonView);
            transform.position = BreakableObjectKeys.DeadObjectPosition;
        }
    }
}