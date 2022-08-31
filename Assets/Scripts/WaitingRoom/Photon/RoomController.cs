using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using FYP.Global;
using FYP.Global.WaitingRoom;
using UnityEngine.SceneManagement;
using FYP.Global.Photon;
using ExitGames.Client.Photon;

namespace FYP.WaitingRoom
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        // bool: isRejoin
        public static Action<bool> onJoinRoom = delegate { };
        public static Action onLeaveRoom = delegate { };
        public static Action onSwitchMasterClient = delegate { };
        // public static Action<Player> onOtherPlayerEnterRoom = delegate { };
        public static Action<Player> onOtherPlayerLeaveRoom = delegate { };

        private void Awake()
        {
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
            LeaveRoomButton.onLeaveRoomButtonClick += handleLeaveRoom;
            StartGameButton.onStartGameButtonClick += handleStartGame;
            PlayerList.onCardHolderCreated += handleJoinedRoom;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
            LeaveRoomButton.onLeaveRoomButtonClick -= handleLeaveRoom;
            StartGameButton.onStartGameButtonClick -= handleStartGame;
            PlayerList.onCardHolderCreated -= handleJoinedRoom;
        }

        private void handleJoinedRoom()
        {
            StartCoroutine(waitForEndOfFrame());

            IEnumerator waitForEndOfFrame()
            {
                yield return new WaitForEndOfFrame();

                UIController.Instance.configureButtomButtons();
                if (SceneController.lastScene == SceneName.InGame)
                {
                    // rejoining after end game

                    onJoinRoom?.Invoke(true);
                    NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, SettingKeys.IsReady, false);
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = true;
                        PhotonNetwork.CurrentRoom.IsVisible = true;
                    }
                }
                else if (SceneController.lastScene == SceneName.MainLobby)
                {
                    // join as a new player
                    initPlayerCustomProperties();
                    onJoinRoom?.Invoke(false);
                }
            }
        }

        private void initPlayerCustomProperties() {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
                { SettingKeys.IsReady, false },
                { SettingKeys.Position, SettingKeys.InitPosition},
                { SettingKeys.TeamNumber, 250 }
            });
        }

        private void handleLeaveRoom() {
            print($"is in room {PhotonNetwork.InRoom}");
            if (PhotonNetwork.InRoom) {
                onLeaveRoom?.Invoke();
                PhotonNetwork.LeaveRoom(true);
            }
        }

        private void handleStartGame() {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonEvents.loadSceneEvent(SceneName.WaitingRoom, SceneName.InGame);
            }
        }

        public override void OnLeftRoom()
        {
            print("Left room successfully");

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer.GetNext());
            }

            SceneController.LoadScene(SceneName.WaitingRoom, SceneName.MainLobby);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print($"{newPlayer.NickName} entered room");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print($"{otherPlayer.NickName} left room");
            onOtherPlayerLeaveRoom?.Invoke(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            print($"New master client is {newMasterClient.NickName}");
            onSwitchMasterClient?.Invoke();
            UIController.Instance.configureButtomButtons();
        }

        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == PhotonCodes.loadSceneEvent) {
                object[] contents = (object[])obj.CustomData;
                SceneController.LoadScene((string)contents[0], (string)contents[1]);
            }
        }
    }
}