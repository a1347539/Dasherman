using FYP.InGame.PlayerInstance;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FYP.InGame.PlayerItemInstance.Consumable
{
    public class HealingPotion : Consumable
    {
        public float healthRecoveryRatio { get; private set; }


        public override void initialize(ScriptablePlayerItem item)
        {
            healthRecoveryRatio = ((ScriptableConsumable)item).healthRecoveryRatio;
            base.initialize(item);
        }

        public override void onSelected(PhotonView pv)
        {
            base.onSelected(pv);
        }

        public override void useItem(PhotonView user, PhotonView receiver = null)
        {
            // called by everyone
            base.useItem(user, receiver);
        }

        protected override void initializeSingleEffect(PhotonView user, PhotonView receiver)
        {
            if (receiver.GetComponent<CharacterItemBehavior>().canTakeItem())
            {
                StartCoroutine(effectBehavior());
            }

            IEnumerator effectBehavior()
            {
                yield return initializeEffect(receiver);
                receiver.GetComponent<CharacterItemBehavior>().takeHealingPotion(healthRecoveryRatio);
            }
        }
        
    }
}