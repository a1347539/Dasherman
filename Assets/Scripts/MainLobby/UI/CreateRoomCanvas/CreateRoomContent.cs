using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Photon.Pun;
using TMPro;
using FYP.Global;

namespace FYP.MainLobby
{
    public class CreateRoomContent : MonoBehaviour
    {
        public static Action<String, GameSettingKeys.GameModes, int> onConfirmCreateRoom = delegate { };

        [SerializeField]
        private GameObject UIMask;

        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Button confirmButton;

        [SerializeField]
        private TMP_InputField RoomNameInputField;

        [SerializeField]
        private ToggleGroup GameModeGroup;

        [SerializeField]
        private ToggleGroup RoomCapacityGroup;

        private String roomName;
        private GameSettingKeys.GameModes gameMode;
        private int roomCapacity;


        private void Awake()
        {
            HomePageContent.onOpenCreateRoomButtonClick += handleSetInputFields;
            HomePageContent.onOpenCreateRoomButtonClick += handleOpenCreateRoomContent;
        }

        private void Start()
        {
            backButton.onClick.AddListener(onCancelButtonClick);
            confirmButton.onClick.AddListener(onConfirmButtonClick);
        }

        private void OnDestroy()
        {
            HomePageContent.onOpenCreateRoomButtonClick -= handleSetInputFields;
            HomePageContent.onOpenCreateRoomButtonClick -= handleOpenCreateRoomContent;

            backButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
        }

        private void handleOpenCreateRoomContent() {
            UIMask.SetActive(true);
            GetComponent<Animator>().SetBool("open", true);
        }

        private void handleSetInputFields() {
            roomName = $"Room {PhotonNetwork.LocalPlayer.NickName}";
            RoomNameInputField.text = roomName;
            gameMode = GameSettingKeys.defaultGameMode;
            roomCapacity = GameSettingKeys.defaultRoomCapacity;
        }

        private void onCancelButtonClick()
        {
            GetComponent<Animator>().SetBool("open", false);
            StartCoroutine(waitForAnimation());

            IEnumerator waitForAnimation()
            {
                yield return new WaitForSeconds(0.5f);
                UIMask.SetActive(false);
            }
        }

        public void onConfirmButtonClick()
        {
            onConfirmCreateRoom?.Invoke(roomName, gameMode, roomCapacity);
        }

        public void onGameModeToggled()
        {
            String s = GameModeGroup.ActiveToggles().FirstOrDefault().name.Substring(0, 3);
            print(s);
            switch (s) {
                case "PvP":
                    gameMode = GameSettingKeys.GameModes.PVP;
                    break;
                case "PvE":
                    gameMode = GameSettingKeys.GameModes.PVE;
                    break;
            }
        }

        public void onRoomCapacityToggled()
        {
            String s = RoomCapacityGroup.ActiveToggles().FirstOrDefault().name.Substring(0, 3);
            print(s);
            switch (s) {
                case "1v1":
                    roomCapacity = 2;
                    break;
                case "3v3":
                    roomCapacity = 6;
                    break;
                case "5v5":
                    roomCapacity = 10;
                    break;
            }
        }
    }
}