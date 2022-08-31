using FYP.Global;
using FYP.Global.InGame;
using FYP.Global.Photon;
using FYP.InGame.AI;
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

namespace FYP.InGame.PlayerInstance
{
    public class CharacterBuilder : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public Action onAnyCharacterAssignComponents = delegate { };

        private CharacterVital vital;
        private CharacterController controller;
        private CharacterAttackController attackController;
        private CharacterAnimationManager animationManager;

        [SerializeField]
        private UIManager uiManager;

        [SerializeField]
        public Transform weaponOffsetTransform;

        [SerializeField]
        private ParticleSystem teleportSmokeEffect;

        public ParticleSystem TeleportSmokeEffect { get { return teleportSmokeEffect; } }

        [SerializeField]
        private GameObject sprite;
        public GameObject Sprite { get { return sprite; } }

        public bool isInitialized = false;

        #region AI
        public AIInput aiInput { get; private set; }
        #endregion


        private void Awake()
        {
            vital = GetComponent<CharacterVital>();
            controller = GetComponent<CharacterController>();
            attackController = GetComponent<CharacterAttackController>();
            animationManager = GetComponent<CharacterAnimationManager>();

            if (!photonView.IsMine) return;
            if (photonView.IsRoomView) return;
            GameManager.onCharacterCreated += handleSetExistingCharacters;
            //GameManager.onOtherCharacterCreated += handleSetNewCharacter;
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            if (photonView.IsRoomView) return;
            GameManager.onCharacterCreated -= handleSetExistingCharacters;
            //GameManager.onOtherCharacterCreated -= handleSetNewCharacter;
        }

        public void initialize(int positionIndex, ScriptableCharacter sc, GameObject weapon, WeaponAttackType attackType)
        {

            List<SpawnPoint> initPoint = MapController.Instance.initPoints.playerSpawnPoints;
            
            Point p;
            if (positionIndex % 2 == 0)
            {
                p = initPoint[positionIndex / 2].getPoint();
            }
            else
            {
                p = initPoint[(positionIndex - 1) / 2 + (initPoint.Count / 2)].getPoint();
            }

            photonView.RPC("initializeRPC", RpcTarget.AllBuffered, p, 
                Mathf.FloorToInt(sc.baseHealth * sc.healthScaling),
                Mathf.FloorToInt(sc.baseMana * sc.manaScaling), 
                sc.baseManaRegenRate * sc.manaRegenScaling, 
                sc.physicalDefenceScaling,
                sc.magicDefenceScaling);
            attackController.initializeStats(sc.baseDashingDamage, sc.physicalDamageScaling, sc.magicDamageScaling, weapon, attackType);
            animationManager.initializeAnimator();
            onAnyCharacterAssignComponents?.Invoke();
        }

        private void handleSetExistingCharacters(int viewID) {

            print($"creating character for {viewID} by {photonView.ViewID}");
            // call by the local player
            if (!InGameTeamManager.Instance.isInTeam(photonView))
            {
                InGameTeamManager.Instance.joinTeam(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code, photonView);
            }

            PhotonView v = PhotonView.Find(viewID);
            if (v == photonView) return;
            if (!InGameTeamManager.Instance.isInTeam(v))
            {
                InGameTeamManager.Instance.joinTeam(v.IsRoomView ? (byte)AIKeys.AITeamNumber : v.Owner.GetPhotonTeam().Code, v);
                v.GetComponent<CharacterVital>().UiManager.TeamIndicator.setTeamColor(
                    PlayerManager.isSameTeam(photonView, v)
                );
            }

/*            List<PhotonView> views = FindObjectsOfType<CharacterBuilder>().ToList().ConvertAll(x => x.photonView);

            foreach (PhotonView view in views)
            {
                if (view.Owner == PhotonNetwork.LocalPlayer) continue;
                if (InGameTeamManager.Instance.isInTeam(view)) continue;
                InGameTeamManager.Instance.joinTeam(view.IsRoomView ? (byte)AIKeys.AITeamNumber : view.Owner.GetPhotonTeam().Code, view);
                print(photonView.ViewID + " " + view.ViewID);
                view.GetComponent<CharacterVital>().UiManager.TeamIndicator.setTeamColor(
                    PlayerManager.isSameTeam(photonView, view)
                );
            }*/
        }

/*        private void handleSetNewCharacter(int viewID, bool isNPC) {
            if (!InGameTeamManager.Instance.isInTeam(((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetPhotonView()))
            {
                InGameTeamManager.Instance.joinTeam(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code, ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetPhotonView());
            }
            print($"creating new character: {viewID} by {photonView.ViewID}");
            PhotonView pv = PhotonView.Find(viewID);
            InGameTeamManager.Instance.joinTeam(isNPC ? (byte)AIKeys.AITeamNumber : pv.Owner.GetPhotonTeam().Code, pv);
            pv.GetComponent<CharacterVital>().UiManager.TeamIndicator.setTeamColor(
                PlayerManager.isSameTeam(photonView, PhotonView.Find(viewID))
                );
        }*/

        [PunRPC]
        private void initializeRPC(Point p, int h, int m, float manaRegenRate, float physicalDefenceScaling, float magicDefenceScaling)
        {
            transform.SetParent(MapObjectManager.Instance.characterContainer);
            controller.setCurrentPoint(p, false);
            vital.initializeStats(h, m, manaRegenRate, physicalDefenceScaling, magicDefenceScaling);
            controller.CharacterState = CharacterController.CharacterStates.idle;
            transform.localScale = MapController.Instance.characterSpriteScaling;
            teleportSmokeEffect.transform.localScale = MapController.Instance.characterSpriteScaling;
            teleportSmokeEffect.transform.position = new Vector3(transform.position.x, transform.position.y + MapController.Instance.characterSpriteOffsetInY / 2);
            isInitialized = true;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (photonView.IsRoomView) return;
            info.Sender.TagObject = this.gameObject;
        }
    }
}