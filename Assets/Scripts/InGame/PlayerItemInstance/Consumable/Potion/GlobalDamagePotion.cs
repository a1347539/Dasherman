using FYP.InGame.PlayerInstance;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FYP.InGame.PlayerItemInstance.Consumable
{
    public class GlobalDamagePotion : Consumable
    {
        public float damageScaling { get; private set; }
        public int duration { get; private set; }

        public override void initialize(ScriptablePlayerItem item)
        {
            damageScaling = ((ScriptableConsumable)item).damageScaling;
            duration = ((ScriptableConsumable)item).duration;
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

        protected override void initializeSingleEffect(PhotonView user, PhotonView target)
        {
            if (target.GetComponent<CharacterItemBehavior>().canTakeItem())
            {
                StartCoroutine(effectBehavior());
            }

            IEnumerator effectBehavior()
            {
                yield return initializeEffect(target);
                target.GetComponent<CharacterItemBehavior>().scaleDamage(damageScaling, duration);
            }
        }
    }
}