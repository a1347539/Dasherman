using FYP.InGame.PlayerInstance;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerItemInstance.Skill
{
    public class Lightning : Skill
    {
        public override void initialize(ScriptablePlayerItem item)
        {
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
                if (user == receiver)
                {
                    yield return initializeSelfEffect(user);
                    user.GetComponent<CharacterItemBehavior>().takeMana(manaCost);
                }
                else
                {
                    yield return initializeOtherPlayerEffect(receiver);
                    receiver.GetComponent<CharacterItemBehavior>().takeDamage(damage, user);
                }
            }
        }
    }
}