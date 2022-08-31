using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using FYP.Global.WaitingRoom;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using FYP.Global.Photon;
using FYP.Global;
using FYP.Global.InGame;

namespace FYP.WaitingRoom
{
    public class PlayerController : PunCallbacksSingleton<PlayerController>
    {

        public static Action<Player, byte> onSwitchTeam = delegate { };

        public static Action<Player, int> onSwitchPosition = delegate { };

        public static Action<Player, Boolean> onToggledIsReady = delegate { };

        public static Action<Boolean> onGameStartable = delegate { };

        private void Awake()
        {
            SetReadyButton.onReady += handleToggleIsReady;
        }

        private void OnDestroy()
        {
            SetReadyButton.onReady -= handleToggleIsReady;
        }

        private void Start()
        {
            setPlayerItems();
        }

        private void handleToggleIsReady(Player player)
        {
            Boolean current = (Boolean)NetworkUtilities.getCustomProperty(player, SettingKeys.IsReady);
            NetworkUtilities.setCustomProperty(player, SettingKeys.IsReady, !current);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            object teamCodeObject;
            if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
            {
                if (teamCodeObject == null) return;
                // print($"switch {targetPlayer.NickName} to {targetPlayer.GetPhotonTeam().Name}");
                onSwitchTeam?.Invoke(targetPlayer, (byte)teamCodeObject);
                return;
            }

            if (changedProps.ContainsKey(SettingKeys.IsReady))
            {
                onToggledIsReady?.Invoke(targetPlayer, (Boolean)NetworkUtilities.getCustomProperty(targetPlayer, SettingKeys.IsReady));
                Player[] players = PhotonNetwork.PlayerList;
                if (players.All(p => (Boolean)NetworkUtilities.getCustomProperty(p, SettingKeys.IsReady)))
                {
                    //there are at least two team which has players
                    if (PhotonTeamsManager.Instance.PhotonTeams.Count(t => PhotonTeamsManager.Instance.GetTeamMembersCount(t.Code) > 0) > 1) {
                        onGameStartable?.Invoke(true);
                        return;
                    }
                }

                onGameStartable?.Invoke(false);
                return;
            }

            if (changedProps.ContainsKey(SettingKeys.Position))
            {

                int newPosition = (int)NetworkUtilities.getCustomProperty(targetPlayer, SettingKeys.Position);
                if (newPosition == -1) { return; }

                // print($"switch {targetPlayer.NickName} to {newPosition} position");

                onSwitchPosition?.Invoke(targetPlayer, newPosition);
                return;
            }
        }

        private void setPlayerItems() {
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
    }
}