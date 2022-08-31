using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using FYP.Global.WaitingRoom;
using FYP.Global;
using System.Linq;
using FYP.Global.Photon;

namespace FYP.WaitingRoom
{
    public class TeamController : PunCallbacksSingleton<TeamController>
    {

        GameSettingKeys.GameModes gameMode;

        private void Awake()
        {
            PlayerCardHolder.onPlayerCardButtonClick += handleSwitchPosition;
            RoomController.onJoinRoom += handleJoinRoom;
            RoomController.onLeaveRoom += handleLeaveRoom;
            RoomController.onOtherPlayerLeaveRoom += handleOtherPlayerLeaveRoom;
            PlayerController.onSwitchTeam += handleSwitchTeam;

        }

        private void OnDestroy()
        {
            PlayerCardHolder.onPlayerCardButtonClick -= handleSwitchPosition;
            RoomController.onJoinRoom -= handleJoinRoom;
            RoomController.onLeaveRoom -= handleLeaveRoom;
            RoomController.onOtherPlayerLeaveRoom -= handleOtherPlayerLeaveRoom;
            PlayerController.onSwitchTeam -= handleSwitchTeam;
        }

        private void initialAssignTeam(Player player)
        {
            // print($"inital team assignment for {player.NickName}");
            switch (PhotonNetwork.CurrentRoom.CustomProperties[GameSettingKeys.GameMode])
            {
                case (int)GameSettingKeys.GameModes.PVP:
                    assignPlayerToTeamForPVP(player);
                    break;
                default:
                    break;
            }
        }

        private List<PhotonTeam> createTeams()
        {
            List<PhotonTeam> photonTeams = new List<PhotonTeam>();
            PhotonHashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            gameMode = (GameSettingKeys.GameModes)properties[GameSettingKeys.GameMode];
            int numberOfTeam = (gameMode == GameSettingKeys.GameModes.PVP) ? GameSettingKeys.teamNumberForPVP : GameSettingKeys.teamNumberForPVE;

            for (int i = 0; i < numberOfTeam; ++i)
            {

                photonTeams.Add(new PhotonTeam
                {
                    Name = $"Team {i + 1}",
                    Code = (byte)(i + 1),
                });
            }
            photonTeams.Add(new PhotonTeam
            {
                Name = $"Team 250",
                Code = (byte)250,
            });
            return photonTeams;
        }

        private void assignPlayerToTeamForPVP(Player player)
        {
            PhotonHashtable playerPropertyTable = new PhotonHashtable();
            List<PlayerCardHolder> holders = UIController.Instance.PlayerCardHolders;
            Player[] players = PhotonNetwork.PlayerList;
            int teamNumber = -1;
            int i = 0;
            for (; i < holders.Count; ++i)
            {
                if (!players.Any(p => (int)p.CustomProperties[SettingKeys.Position] == i)) {
                    playerPropertyTable[SettingKeys.Position] = i;
                    teamNumber = (i % 2 == 0) ? 1 : 2;
                    break;
                }
            }
            if (teamNumber == -1) {
                print("error, room full but still joined");
                return;
            }
            PhotonTeam team;
            if (PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamNumber, out team))
            {
                player.SwitchTeam(team);
                playerPropertyTable[SettingKeys.TeamNumber] = teamNumber;
            }
            else {
                print($"error, team {teamNumber} does not exist");
            }
            player.SetCustomProperties(playerPropertyTable);
        }

        private void updateTeam() {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                PhotonTeam playerTeam = player.GetPhotonTeam();
                if (playerTeam != null)
                {
                    PhotonTeamsManager.Instance.PlayersPerTeam[playerTeam.Code].Add(player);
                }
            }
        }

        private void handleJoinRoom(bool isRejoin)
        {
            if (isRejoin) return;
            PhotonTeamsManager.Instance.PhotonTeams = createTeams();
            updateTeam();
            PhotonTeam team;
            PhotonTeamsManager.Instance.TryGetTeamByCode(250, out team);
            PhotonNetwork.LocalPlayer.JoinTeam(team);
        }

        private void handleLeaveRoom()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, SettingKeys.IsReady, false);
        }

        private void handleOtherPlayerLeaveRoom(Player player)
        {

        }

        private void handleSwitchPosition(int position)
        {
            if ((Boolean)NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, SettingKeys.IsReady)) {
                return;
            }
            byte teamCodeToSwitch = 1;
            if (gameMode == GameSettingKeys.GameModes.PVP)
            {
                teamCodeToSwitch = (position % 2 == 0) ? (byte)1 : (byte)2;
            }
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, SettingKeys.Position, position);
            if (gameMode == GameSettingKeys.GameModes.PVE)
            {
                return;
            }
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != teamCodeToSwitch)
            {
                PhotonNetwork.LocalPlayer.SwitchTeam(teamCodeToSwitch);
            }
        }

        private void handleSwitchTeam(Player player, byte teamCode)
        {
            PhotonTeam newTeam = PhotonTeamsManager.Instance.PhotonTeams.Single(t => t.Code == teamCode);
            if (newTeam == null) return;

            if (newTeam.Code == 250)
            {
                initialAssignTeam(player);
                return;
            }
        }

        public void debugPrint() {
            foreach (PhotonTeam team in PhotonTeamsManager.Instance.PhotonTeams)
            {
                Player[] players;
                PhotonTeamsManager.Instance.TryGetTeamMembers(team, out players);
                print($"{team.Name} has {players.Length} players");
                foreach (Player player in players)
                {
                    print($"{team.Name} has {player.NickName} at {NetworkUtilities.getCustomProperty(player, SettingKeys.Position)} with {player.TagObject == null}");
                }
            }
        }

    }
}
