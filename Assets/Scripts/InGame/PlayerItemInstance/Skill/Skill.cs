using FYP.Global.Photon;
using FYP.InGame.PlayerItemInstance;
using FYP.InGame.PlayerItemInstance.Consumable;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global;
using FYP.InGame.PlayerInstance;
using UnityEngine.Rendering;
using FYP.InGame.Map;
using System;
using FYP.InGame.Photon;
using Random = UnityEngine.Random;

namespace FYP.InGame.PlayerItemInstance.Skill
{
    public class Skill : MonoBehaviourPun, PlayerUsable
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

        public int damage { get; protected set; }
        public int manaCost { get; protected set; }
        public int cooldownDuration { get; protected set; }
        private float currentCooldownTime;

        protected bool onCooldown = false;
        protected GameObject ownerCharacter;
        protected int indexInItemWheel;

        private void Start()
        {
            ownerCharacter = ((GameObject)PhotonNetwork.LocalPlayer.TagObject);
            indexInItemWheel = PlayerItemManager.Instance.localPlayerItems.IndexOf(GetComponent<PlayerUsable>());
        }

        private void Update()
        {
            if (manaCost > ownerCharacter.GetComponent<CharacterVital>().currentMana)
            {
                if (PlayerItemManager.Instance.itemWheel.wheelSlots[indexInItemWheel].Interactable)
                {
                    PlayerItemManager.Instance.itemWheel.disableSlot(indexInItemWheel);
                }
            }
            if (onCooldown)
            {
                currentCooldownTime -= Time.deltaTime;
                if (currentCooldownTime <= 0) { onCooldown = false; }
                string displayTimeText = currentCooldownTime <= 0 ? String.Empty : Mathf.FloorToInt(currentCooldownTime).ToString();
                PlayerItemManager.Instance.itemWheel.wheelSlots[indexInItemWheel].cooldownTimer.text = displayTimeText;
            }
            else if (manaCost <= ownerCharacter.GetComponent<CharacterVital>().currentMana && !onCooldown)
            {
                if (!PlayerItemManager.Instance.itemWheel.wheelSlots[indexInItemWheel].Interactable)
                {
                    PlayerItemManager.Instance.itemWheel.enableSlot(indexInItemWheel);
                }
            }
        }

        virtual public void initialize(ScriptablePlayerItem item)
        {
            transform.SetParent(PlayerItemManager.Instance.globalPlayerItemContainer);
            playerItemType = PlayerUsableType.Skill;
            playerItemID = (byte)item.playerItemID;
            playerItemName = item.playerItemName;
            playerItemDescription = item.playerItemDescription;
            target = item.target;

            damage = ((ScriptableSkill)item).damage;
            manaCost = ((ScriptableSkill)item).manaCost;
            cooldownDuration = ((ScriptableSkill)item).cooldownTime;

            PlayerItemManager.Instance.localPlayerItems.Add(this);
        }

        public virtual void onSelected(PhotonView pv)
        {
            PhotonView receiver = null;
            if (target == PlayerUsableTarget.random)
            {
                List<InGameTeam> enemyteams = InGameTeamManager.Instance.getAllTeams().Where(team =>
                team.code != InGameTeamManager.Instance.getTeamByPhotonView(pv).code
                ).ToList();

                List<PhotonView> enemies = new List<PhotonView>();
                foreach (InGameTeam team in enemyteams)
                {
                    enemies.AddRange(team.views);
                }

                receiver = enemies[Random.Range(0, enemies.Count)];
            }
            PhotonEvents.InGameEvents.useItemEvent((byte)playerItemType, playerItemID, (byte)target, pv.ViewID, receiver == null ? null : receiver.ViewID);
        }

        public virtual void useItem(PhotonView user, PhotonView receiver = null)
        {
            // called by everyone
            initializeSingleEffect(user, user);

            Action afterCasting = () =>
            {
                if (target == PlayerUsableTarget.all)
                {
                    initializeTargetEffect(user);
                }
                else if (target == PlayerUsableTarget.random)
                {
                    initializeTargetEffect(user, receiver);
                }
            };
            StartCoroutine(waitUntilAfterCastingAnimation());
            IEnumerator waitUntilAfterCastingAnimation()
            {
                yield return new WaitForSeconds(1f);
                user.GetComponent<CharacterItemBehavior>().itemAnimationFinished();
                afterCasting.Invoke();
            }
        }

        protected virtual void initializeSingleEffect(PhotonView user, PhotonView target)
        {

        }

        protected bool initializeSelfEffect(PhotonView pv)
        {
            // do casting skill animation
            pv.GetComponent<PlayerInstance.CharacterController>().CharacterState = PlayerInstance.CharacterController.CharacterStates.useSkill;

            if (pv.Owner == PhotonNetwork.LocalPlayer && !pv.IsRoomView)
            {
                StartCoroutine(waitUntilAfterCasting());
                currentCooldownTime = cooldownDuration;
                onCooldown = true;
                PlayerItemManager.Instance.itemWheel.disableSlot(indexInItemWheel);

                IEnumerator waitUntilAfterCasting()
                {
                    yield return new WaitForSeconds(1f);
                    PlayerItemManager.Instance.itemWheel.enableAllSlots();
                    PlayerItemManager.Instance.itemWheel.disableSlot(indexInItemWheel);
                }
            }

            StartCoroutine(destroyEffectCoroutine());

            IEnumerator destroyEffectCoroutine()
            {
                yield return new WaitForSeconds(1);
                pv.GetComponent<CharacterItemBehavior>().itemAnimationFinished();
            }

            return true;
        }

        protected bool initializeOtherPlayerEffect(PhotonView pv)
        {
            GameObject effect = Instantiate(targetAnimationObject, animationContainer);
            effect.transform.position = pv.transform.position;

            effect.GetComponent<SpriteRenderer>().sortingOrder =
                pv.GetComponent<CharacterBuilder>().Sprite.GetComponent<SortingGroup>().sortingOrder;
            effect.transform.localScale = MapController.Instance.characterSpriteScaling;
            effect.SetActive(true);
            StartCoroutine(destroyEffectCoroutine(effect, () => { pv.GetComponent<CharacterItemBehavior>().itemAnimationFinished(); }));

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
                List<InGameTeam> enemyteams = InGameTeamManager.Instance.getAllTeams().Where(team => 
                team.code != InGameTeamManager.Instance.getTeamByPhotonView(user).code
                ).ToList();

                List<PhotonView> enemies = new List<PhotonView>();
                foreach (InGameTeam team in enemyteams) {
                    enemies.AddRange(team.views);
                }

                foreach (PhotonView pv in enemies)
                {
                    initializeSingleEffect(user, pv);
                }
            }
        }
    }
}