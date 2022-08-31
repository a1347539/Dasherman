using ExitGames.Client.Photon;
using FYP.Global;
using FYP.Global.InGame;
using FYP.Global.Photon;
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
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterVital : MonoBehaviourPun, IDamageable
    {
        public Action<float> onSetHealth = delegate { };
        public Action<float> onSetMana = delegate { };

        [SerializeField]
        private GameObject debugButton;

        [SerializeField]
        private UIManager uiManager;
        public UIManager UiManager { get { return uiManager; } }

        [SerializeField]
        private SpriteRenderer avatarSpriteRenderer;

        [SerializeField]
        private CharacterHealthBar healthBar;

        private CharacterController controller;
        private CharacterBuilder builder;

        public int maxHealth { get; private set; }
        public int maxMana { get; private set; }
        public int currentHealth { get; private set; }
        public int currentMana;

        public float currentManaRaw;

        public float rawPhysicalDefenceScaling;
        public float PhysicalDefenceScaling
        {
            get {
                if (rawPhysicalDefenceScaling > 7) { return 7; }
                else { return rawPhysicalDefenceScaling; }
            }
        }

        public float rawMagicDefenceScaling;
        public float MagicDefenceScaling
        {
            get
            {
                if (rawMagicDefenceScaling > 8) { return 8; }
                else { return rawMagicDefenceScaling; }
            }
        }

        public float manaRegenerationRate { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            builder = GetComponent<CharacterBuilder>();
        }

        private void Start()
        {
            if (!photonView.IsMine) return;
            debugButton = GameObject.FindGameObjectWithTag("characterDebug");
            debugButton.GetComponent<Button>().onClick.AddListener(debugTryFunction);
        }

        public void initializeStats(int h, int m, float mRegen, float physicaldefenceScaling, float maigcDefenceScaling) {
            maxHealth = h;
            currentHealth = h;
            maxMana = m;
            currentMana = m;
            currentManaRaw = m;
            manaRegenerationRate = mRegen;
            this.rawPhysicalDefenceScaling = physicaldefenceScaling;
            this.rawMagicDefenceScaling = maigcDefenceScaling;
        }

        public void rechargeMana() {
            if (currentMana >= maxMana) {
                currentMana = maxMana;
                return;
            }
            float deltaManaRawValue = manaRegenerationRate * Time.deltaTime;
            currentManaRaw += deltaManaRawValue;
            currentMana = (int)currentManaRaw;
            onSetMana?.Invoke(currentManaRaw / maxMana);
        }

        public void setHealth(int deltaHealth, PhotonView pv = null) {
            // called by incident object from caller's instance
            print("sethealth");
            int newHealth = currentHealth + deltaHealth;
            if (newHealth > maxHealth) { newHealth = maxHealth; }
            photonView.RPC("setHealthRPC", RpcTarget.All, new object[] { newHealth });
            if (newHealth <= 0)
            {
                photonView.RPC("onDie", RpcTarget.All);
                if (pv != null)
                    NetworkUtilities.setCustomProperty(pv.Owner, PlayerKeys.InGameScore, ((int)NetworkUtilities.getCustomProperty(pv.Owner, PlayerKeys.InGameScore)) + 1);
            }
        }

        public void setMana(int deltaMana) {
            float newMana = currentManaRaw + deltaMana;
            if (newMana > maxMana) { newMana = maxMana; }
            currentManaRaw = newMana;
            currentMana = (int)newMana;
            onSetMana?.Invoke(currentManaRaw / (float)maxMana);
        }

        void IDamageable.takeDamage(int damage, DamageType damageType, PhotonView pv)
        {
            int damageAfterDefence;
            if (damageType == DamageType.skillHit)
            {
                damageAfterDefence = Mathf.FloorToInt(damage * (1 - MagicDefenceScaling / 10));
            }
            else
            {
                damageAfterDefence = Mathf.FloorToInt(damage * (1 - PhysicalDefenceScaling / 10));
                if (damageType == DamageType.physicalHit)
                {
                    photonView.RPC("takeDamageEffectRPC", RpcTarget.All);
                }
                else if (damageType == DamageType.dashing)
                {
                    int respawnX;
                    int respawnY;
                    do
                    {
                        print("get spawn point");
                        respawnX = Random.Range(0, MapController.Instance.playableMapSize.x);
                        respawnY = Random.Range(0, MapController.Instance.playableMapSize.y);
                    }
                    while (MapController.Instance.tileMatrix[respawnY][respawnX].tileState != Tile.TileStates.empty);

                    Vector2 spawnPosition = MapController.Instance.tileMatrix[respawnY][respawnX].worldPositionOfCellCenter;

                    photonView.RPC("respawnTeleportRPC", RpcTarget.All, new object[] { new Point(respawnX, respawnY), spawnPosition });
                }
            }

            print($"{currentHealth} {damageAfterDefence}");

            setHealth(-damageAfterDefence, pv);
        }

        IEnumerator respawnTeleportCoroutine(Point spawnPoint, Vector2 spawnPosition) {
            uiManager.deactivateAllUI();

            builder.TeleportSmokeEffect.Play();

            while (builder.TeleportSmokeEffect.isPlaying) {
                yield return null;
            }
            if (controller.CharacterState == CharacterController.CharacterStates.died) yield break;
            // free up tile from current player
            MapController.Instance.tileMatrix[controller.currentPoint.y][controller.currentPoint.x].objectExit(photonView);
            transform.position = new Vector3(
                spawnPosition.x,
                spawnPosition.y + MapController.Instance.characterSpriteOffsetInY
                );
            yield return new WaitForSeconds(2f);

            controller.setCurrentPoint(spawnPoint, false);

            uiManager.activateAllUI();
            controller.CharacterState = CharacterController.CharacterStates.idle;
        }

        IEnumerator TakeDamageEffect() {
            yield return new WaitForSeconds(0.1f);
            avatarSpriteRenderer.color = new Color(0.8f, 0.3f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            avatarSpriteRenderer.color = Color.white;
        }



        [PunRPC]
        private void respawnTeleportRPC(Point spawnPoint, Vector2 spawnPosition) {
            controller.CharacterState = CharacterController.CharacterStates.respawning;
            StartCoroutine(respawnTeleportCoroutine(spawnPoint, spawnPosition));
        }

        [PunRPC]
        private void takeDamageEffectRPC() {
            StartCoroutine(TakeDamageEffect());
        }

        [PunRPC]
        private void setHealthRPC(int newHealth) {
            currentHealth = newHealth;
            healthBar.setHealth(newHealth / (float)maxHealth);
            onSetHealth?.Invoke(newHealth / (float)maxHealth);
        }

        [PunRPC]
        private void onDie() {
            controller.CharacterState = CharacterController.CharacterStates.died;
        }

        public void debugTryFunction() {
            GetComponent<IDamageable>().takeDamage(30, DamageType.physicalHit);
        }
    }
}