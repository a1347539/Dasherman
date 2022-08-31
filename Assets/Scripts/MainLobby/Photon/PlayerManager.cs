using FYP.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FYP.MainLobby
{
    public class PlayerManager : MonoBehaviour
    {
        void Start()
        {
            NetworkManager.onGetPlayerClass += handleGetPlayerClass;
        }

        private void OnDestroy()
        {
            NetworkManager.onGetPlayerClass -= handleGetPlayerClass;
        }

        private void handleGetPlayerClass(int characterClassID) {
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerGlobalCustomProperties.PlayerClassID, characterClassID);
        }
    }
}