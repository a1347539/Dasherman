using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System;
using FYP.Global;

namespace FYP.MainLobby
{
    public class RoomCard : MonoBehaviour
    {
        public static Action<RoomInfo> onRoomCardClick = delegate { };

        [SerializeField]
        private TMP_Text roomNameText;
        [SerializeField]
        private TMP_Text gameModeText;
        [SerializeField]
        private TMP_Text roomCapacityText;

        public RoomInfo roomInfo { get; private set; }

        public void initializeRoomCard(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            roomNameText.text = roomInfo.Name;
            gameModeText.text = ((GameSettingKeys.GameModes)roomInfo.CustomProperties[GameSettingKeys.GameMode]).ToString();
            roomCapacityText.text = $"1/{roomInfo.MaxPlayers}";
        }

        public void updateRoomCard(int currentPlayer) {
            roomCapacityText.text = $"{currentPlayer}/{roomInfo.MaxPlayers}";
        }

        public void onClick() {
            onRoomCardClick?.Invoke(roomInfo);
        }
    }
}