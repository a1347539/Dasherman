using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

namespace FYP.WaitingRoom
{
    public class PlayerList : MonoBehaviour
    {
        public static Action onCardHolderCreated = delegate { };

        [SerializeField]
        private GameObject playerCardHolderUI;

        private void Awake()
        {
            WaitingRoomManager.onDataReady += handleCreateCradHolders;
        }

        private void OnDestroy()
        {
            WaitingRoomManager.onDataReady -= handleCreateCradHolders;
        }

        private void handleCreateCradHolders()
        {
            for (int i = 0; i < 10; ++i)
            {
                PlayerCardHolder holder = Instantiate(playerCardHolderUI, this.transform).GetComponent<PlayerCardHolder>();
                UIController.Instance.PlayerCardHolders.Add(holder);
                int stateIndex;
                if (i >= PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    stateIndex = 2;
                }
                else
                {
                    stateIndex = 1;
                }
                holder.initialize(stateIndex, i);
            }

            onCardHolderCreated?.Invoke();
        }
    }
}