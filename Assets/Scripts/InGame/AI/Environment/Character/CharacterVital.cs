using FYP.InGame;
using FYP.InGame.AI.Agent;
using FYP.InGame.Map;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterVital : MonoBehaviour, IDamageable
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
            get
            {
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

        public void initializeStats(int h, int m, float mRegen, float physicaldefenceScaling, float maigcDefenceScaling)
        {
            maxHealth = h;
            currentHealth = h;
            maxMana = m;
            currentMana = m;
            currentManaRaw = m;
            manaRegenerationRate = mRegen;
            this.rawPhysicalDefenceScaling = physicaldefenceScaling;
            this.rawMagicDefenceScaling = maigcDefenceScaling;
        }

        public void rechargeMana(int customRate)
        {
            if (currentMana >= maxMana)
            {
                currentMana = maxMana;
                return;
            }
            float deltaManaRawValue = manaRegenerationRate * Time.deltaTime * customRate;
            currentManaRaw += deltaManaRawValue;
            currentMana = (int)currentManaRaw;
            onSetMana?.Invoke(currentManaRaw / maxMana);

            // AI Training
            print($"add reward {deltaManaRawValue}");
            GetComponent<DashingGameAgent>().AddReward(deltaManaRawValue);
        }

        public void setHealth(int deltaHealth)
        {
            // called by incident object from caller's instance
            print("sethealth");
            int newHealth = currentHealth + deltaHealth;
            if (newHealth > maxHealth) { newHealth = maxHealth; }

            currentHealth = newHealth;
            healthBar.setHealth(newHealth / (float)maxHealth);
            onSetHealth?.Invoke(newHealth / (float)maxHealth);

            if (newHealth <= 0)
            {
                controller.CharacterState = CharacterController.CharacterStates.died;
            }
        }

        public void setMana(int deltaMana)
        {
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
                    StartCoroutine(TakeDamageEffect());
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

                    controller.CharacterState = CharacterController.CharacterStates.respawning;
                    StartCoroutine(respawnTeleportCoroutine(new Point(respawnX, respawnY), spawnPosition));
                }
            }

            print($"{currentHealth} {damageAfterDefence}");

            setHealth(-damageAfterDefence);
        }

        IEnumerator TakeDamageEffect()
        {
            yield return new WaitForSeconds(0.1f);
            avatarSpriteRenderer.color = new Color(0.8f, 0.3f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            avatarSpriteRenderer.color = Color.white;
        }

        IEnumerator respawnTeleportCoroutine(Point spawnPoint, Vector2 spawnPosition)
        {
            uiManager.deactivateAllUI();

            // free up tile from current player
            MapController.Instance.tileMatrix[controller.currentPoint.y][controller.currentPoint.x].objectExit(gameObject);

            yield return new WaitForSeconds(0.5f);

            if (controller.CharacterState == CharacterController.CharacterStates.died) yield break;

            transform.position = new Vector3(
                spawnPosition.x,
                spawnPosition.y + MapController.Instance.characterSpriteOffsetInY
                );
            yield return new WaitForSeconds(2f);

            controller.setCurrentPoint(spawnPoint, false);

            uiManager.activateAllUI();
            controller.CharacterState = CharacterController.CharacterStates.idle;
        }
    }
}