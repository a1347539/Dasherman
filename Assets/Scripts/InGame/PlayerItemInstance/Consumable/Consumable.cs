using ExitGames.Client.Photon;
using FYP.Global;
using FYP.Global.Photon;
using FYP.InGame.Map;
using FYP.InGame.Photon;
using FYP.InGame.PlayerInstance;
using FYP.InGame.PlayerItemInstance.Skill;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace FYP.InGame.PlayerItemInstance.Consumable
{
    public class Consumable : MonoBehaviourPun, PlayerUsable
    {
        [SerializeField]
        private GameObject iconPrefab;
        [SerializeField]
        protected GameObject selfAnimationObject;
        [SerializeField]
        protected GameObject targetAnimationObject;
        [SerializeField]
        protected Transform animationContainer;

        public GameObject IconPrefab => iconPrefab;

        public PlayerUsableType playerItemType { get; private set; }
        public byte playerItemID { get; private set; }
        public string playerItemName { get; private set; }
        public string playerItemDescription { get; private set; }
        public PlayerUsableTarget target { get; private set; }

        protected int indexInItemWheel;

        private void Start()
        {
            indexInItemWheel = PlayerItemManager.Instance.localPlayerItems.IndexOf(GetComponent<PlayerUsable>());
        }

        public virtual void initialize(ScriptablePlayerItem item)
        {
            transform.SetParent(PlayerItemManager.Instance.globalPlayerItemContainer);
            playerItemType = PlayerUsableType.Consumable;
            playerItemID = (byte)item.playerItemID;
            playerItemName = item.playerItemName;
            playerItemDescription = item.playerItemDescription;
            target = item.target;
            PlayerItemManager.Instance.localPlayerItems.Add(this);
        }

        public virtual void onSelected(PhotonView pv) {
            PhotonView receiver = null;
            if (target == PlayerUsableTarget.random)
            {
                List<PhotonView> teammates = InGameTeamManager.Instance.getTeamByPhotonView(pv).views;
                receiver = teammates[Random.Range(0, teammates.Count)];
            }
            PhotonEvents.InGameEvents.useItemEvent((byte)playerItemType, playerItemID, (byte)target, pv.ViewID, receiver == null ? null : receiver.ViewID);
        }

        public virtual void useItem(PhotonView user, PhotonView receiver = null) {
            // called by everyone
            if (user.Owner == PhotonNetwork.LocalPlayer && !user.IsRoomView) {
                PlayerItemManager.Instance.itemWheel.wheelSlots[indexInItemWheel].decreaseItemAmount();
            }

            initializeSingleEffect(user, user);
            if (target == PlayerUsableTarget.all)
            {
                initializeTargetEffect(user);
            }
            else if (target == PlayerUsableTarget.random)
            {
                initializeTargetEffect(user, receiver);
            }
        }

        protected virtual void initializeSingleEffect(PhotonView user, PhotonView target) { 
        
        }

        protected virtual bool initializeEffect(PhotonView pv) {
            GameObject effect = Instantiate(selfAnimationObject, animationContainer);
            effect.transform.position = pv.transform.position;
            
            effect.GetComponent<SpriteRenderer>().sortingOrder =
                pv.GetComponent<CharacterBuilder>().Sprite.GetComponent<SortingGroup>().sortingOrder;
            effect.transform.localScale = MapController.Instance.characterSpriteScaling;
            effect.SetActive(true);
            StartCoroutine(destroyEffectCoroutine(effect, () => { PlayerItemManager.Instance.itemWheel.enableAllSlots(); pv.GetComponent<CharacterItemBehavior>().itemAnimationFinished(); }));

            IEnumerator destroyEffectCoroutine(GameObject effect, Action onDestroy)
            {
                while (effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                {
                    yield return null;
                }
                Destroy(effect);
                onDestroy?.Invoke();
            }
            return true;
        }

        private void initializeTargetEffect(PhotonView user, PhotonView target = null)
        {
            if (target == null)
            {
                List<PhotonView> teammates = InGameTeamManager.Instance.getTeamByPhotonView(user).views;

                foreach (PhotonView pv in teammates)
                {
                    if (pv == user) continue;
                    initializeSingleEffect(user, pv);
                }
            }
        }
    }
}