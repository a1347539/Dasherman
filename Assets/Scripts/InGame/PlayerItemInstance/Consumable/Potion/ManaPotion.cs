using FYP.InGame.PlayerInstance;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerItemInstance.Consumable
{
    public class ManaPotion : Consumable
    {
        public float manaRecoveryRatio { get; private set; }


        public override void initialize(ScriptablePlayerItem item)
        {
            manaRecoveryRatio = ((ScriptableConsumable)item).manaRecoveryRatio;
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
                receiver.GetComponent<CharacterItemBehavior>().takeManaPotion(manaRecoveryRatio);
            }
        }
    }
}