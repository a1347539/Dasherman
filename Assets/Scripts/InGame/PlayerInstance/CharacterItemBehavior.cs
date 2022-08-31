using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterItemBehavior : MonoBehaviourPun
    {
        enum IconIndex { 
            Damage = 0,
            Defence = 1,
        }

        [SerializeField]
        private Transform BuffIconContainer;

        private CharacterVital vital;
        private CharacterController controller;
        private CharacterAttackController attackController; 

        [SerializeField]
        private List<GameObject> buffIcons = new List<GameObject>();

        [SerializeField]
        private List<GameObject> debuffIcons = new List<GameObject>();

        private void Awake()
        {
            vital = GetComponent<CharacterVital>();
            controller = GetComponent<CharacterController>();
            attackController = GetComponent<CharacterAttackController>();
        }

        public bool canTakeItem() {
            if (controller.CharacterState == CharacterController.CharacterStates.idle ||
                controller.CharacterState == CharacterController.CharacterStates.aiming ||
                controller.CharacterState == CharacterController.CharacterStates.attacking
                ) {
                return true;
            }
            return false;
        }

        #region consumable behavior

        public void takeHealingPotion(float healRecoveryRatio)
        {
            // called by everyone
            if (!photonView.IsMine) return;
            controller.CharacterState = CharacterController.CharacterStates.takeItemEffect;
            vital.setHealth(Mathf.FloorToInt(vital.maxHealth * healRecoveryRatio));
        }

        public void takeManaPotion(float manaRecoveryRatio) {
            if (!photonView.IsMine) return;
            controller.CharacterState = CharacterController.CharacterStates.takeItemEffect;
            vital.setMana(Mathf.FloorToInt(vital.maxMana * manaRecoveryRatio));
        }

        public void scaleDamage(float damageScaling, int duration) {
            if (!photonView.IsMine) return;
            controller.CharacterState = CharacterController.CharacterStates.takeItemEffect;
            attackController.physicalDamageScaling += damageScaling;
            attackController.magicDamageScaling += damageScaling;
            GameObject icon = null;
            if (!photonView.IsRoomView)
            {
                if (damageScaling > 0)
                    icon = Instantiate(buffIcons[(int)IconIndex.Damage], BuffIconContainer);
                else
                    icon = Instantiate(debuffIcons[(int)IconIndex.Damage], BuffIconContainer);
            }
            StartCoroutine(endDamageEffect());
            IEnumerator endDamageEffect() { 
                yield return new WaitForSeconds(duration);
                attackController.physicalDamageScaling -= damageScaling;
                attackController.magicDamageScaling -= damageScaling;
                if (!photonView.IsRoomView)
                {
                    Destroy(icon);
                }
            }
        }

        public void scaleDefence(float defenceScaling, int duration)
        {
            if (!photonView.IsMine) return;
            controller.CharacterState = CharacterController.CharacterStates.takeItemEffect;
            vital.rawPhysicalDefenceScaling += defenceScaling;
            vital.rawMagicDefenceScaling += defenceScaling;
            GameObject icon = null;
            if (!photonView.IsRoomView)
            {
                if (defenceScaling > 0)
                    icon = Instantiate(buffIcons[(int)IconIndex.Defence], BuffIconContainer);
                else
                    icon = Instantiate(debuffIcons[(int)IconIndex.Defence], BuffIconContainer);
            }
            StartCoroutine(endDamageEffect());
            IEnumerator endDamageEffect()
            {
                yield return new WaitForSeconds(duration);
                vital.rawPhysicalDefenceScaling -= defenceScaling;
                vital.rawMagicDefenceScaling -= defenceScaling;
                if (!photonView.IsRoomView)
                {
                    Destroy(icon);
                }
            }
        }

        #endregion

        #region skill behavior
        public void takeDamage(int damage, PhotonView pv) {
            if (photonView.IsMine)
            {
                float scaling = pv.gameObject.GetComponent<CharacterAttackController>().magicDamageScaling;
                controller.CharacterState = CharacterController.CharacterStates.takeItemEffect;
                GetComponent<IDamageable>().takeDamage(Mathf.FloorToInt(damage * scaling), DamageType.skillHit, pv.IsRoomView ? null : pv);
            } 
        }

        public void takeMana(int manaCost) {
            if (photonView.IsMine)
            {
                vital.setMana(-manaCost);
            }
        }

        #endregion

        public void itemAnimationFinished() {
            if (controller.CharacterState == CharacterController.CharacterStates.died) return;
            IEnumerator waitTilIdle() {
                while (controller.CharacterState == CharacterController.CharacterStates.respawning) { 
                    yield return null;
                }
                controller.CharacterState = CharacterController.CharacterStates.idle;
            }

            StartCoroutine(waitTilIdle());
        }
    }
}