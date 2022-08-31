using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FYP.Global.InGame;
using FYP.Global;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using FYP.InGame.PlayerInstance;

namespace FYP.InGame.Photon
{
    public class PlayerManager : PunCallbacksSingleton<PlayerManager>
    {
        public static Action onPlayerItemLoaded = delegate { };

        public static bool isSameTeam(PhotonView myView, PhotonView otherView)
        {
            return InGameTeamManager.Instance.getTeamByPhotonView(myView).code ==
                InGameTeamManager.Instance.getTeamByPhotonView(otherView).code;
        }
        private void Awake()
        {
            RoomController.onDebugJoin += handleSetSkillProperties;
            RoomController.onNormalJoin += handleNormalJoin;
            RoomController.onJoinGame += handleJoinGame;
            GameManager.onExitInGameScene += handleRemovePlayerTagObject;
        }

        private void OnDestroy()
        {
            RoomController.onDebugJoin -= handleSetSkillProperties;
            RoomController.onNormalJoin -= handleNormalJoin;
            RoomController.onJoinGame -= handleJoinGame;
            GameManager.onExitInGameScene -= handleRemovePlayerTagObject;
        }

        private void handleJoinGame()
        {
            // score is the number of enemy killed
            print("setting score");
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerKeys.InGameScore, 0);
        }

        private void handleNormalJoin() {
            onPlayerItemLoaded?.Invoke();
        }

        private void handleSetSkillProperties() {
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerConsumableIDs, new byte[] {
                (byte)PlayerItemID.ConsumableID.ManaPotion,
                (byte)PlayerItemID.ConsumableID.GlobalDamagePotion
            });
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerConsumableAmounts, new byte[] {
                2, 2
            });
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerSkillIDs, new byte[] {
                (byte)PlayerItemID.SkillID.Explosion,
                (byte)PlayerItemID.SkillID.Lightning,
                (byte)PlayerItemID.SkillID.IcePillar
            }); ;
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerSkillAmounts, new byte[] {
                1, 1, 1
            });
        }

        private void handleRemovePlayerTagObject() {
            foreach (Player player in PhotonNetwork.PlayerList) {
                player.TagObject = null;
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps.ContainsKey(PlayerItemKeys.PlayerSkillAmounts))
                {
                    onPlayerItemLoaded?.Invoke();
                }
            }
        }
    }
}