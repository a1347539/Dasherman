using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.MainLobby
{
    public class HomePageContent : MonoBehaviour
    {
        public static Action onOpenRoomListButtonClick = delegate { };
        public static Action onOpenCreateRoomButtonClick = delegate { };


        [SerializeField]
        private Button displayRoomListButton;
        [SerializeField]
        private Button createRoomButton;

        private void Start()
        {
            displayRoomListButton.onClick.AddListener(handleOpenRoomListContent);
            createRoomButton.onClick.AddListener(handleOpenCreateRoomContent);
        }

        private void OnDestroy()
        {
            displayRoomListButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.RemoveAllListeners();
        }

        private void handleOpenRoomListContent() {
            onOpenRoomListButtonClick?.Invoke();
        }

        private void handleOpenCreateRoomContent() {
            onOpenCreateRoomButtonClick?.Invoke();
        }
    }
}