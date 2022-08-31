using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using FYP.Global;

namespace FYP.MainLobby
{
    public class RoomController : MonoBehaviourPunCallbacks
    {
        public static Action<RoomInfo> onOtherRoomCreated = delegate { };
        public static Action<RoomInfo> onOtherRoomRemoved = delegate { };

        private void Awake()
        {
            // CreateRoomButton.onCreateRoomButtonClick += handleCreateRoom;
            RoomCard.onRoomCardClick += handleUserJoinRoom;
            CreateRoomContent.onConfirmCreateRoom += handleCreateRoom;
        }

        private void OnDestroy()
        {
            // CreateRoomButton.onCreateRoomButtonClick -= handleCreateRoom;
            RoomCard.onRoomCardClick -= handleUserJoinRoom;
            CreateRoomContent.onConfirmCreateRoom -= handleCreateRoom;
        }

        private void handleCreateRoom(String roomName, GameSettingKeys.GameModes gameMode, int roomCapacity) {
            if (!PhotonNetwork.IsConnected) { return; }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)roomCapacity;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { GameSettingKeys.GameMode };
            roomOptions.CustomRoomProperties = new PhotonHashtable() {
                { GameSettingKeys.GameMode, gameMode }
            };

            print($"creating room {roomName}");
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);

        }

        private void handleUserJoinRoom(RoomInfo roomInfo) {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }

        public override void OnJoinedRoom()
        {
            print("Joined room successfully");
            SceneController.LoadScene(SceneName.MainLobby, SceneName.WaitingRoom);
        }

        public override void OnCreatedRoom()
        {
            // print("Created room successfully");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            print($"Created room failed: {message}");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList)
                {
                    onOtherRoomRemoved?.Invoke(info);
                }
                else
                {
                    onOtherRoomCreated?.Invoke(info);
                }
            }
        }
    }
}