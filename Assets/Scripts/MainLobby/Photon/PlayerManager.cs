using FYP.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PlayFab.ClientModels;

namespace FYP.MainLobby
{
    public class PlayerManager : MonoBehaviour
    {
        void Start()
        {
            NetworkManager.onGetPlayerClass += handleSetPlayerClassId;
        }

        private void OnDestroy()
        {
            NetworkManager.onGetPlayerClass -= handleSetPlayerClassId;
        }

        private void handleSetPlayerClassId(Dictionary<string, UserDataRecord> keyValuePairs) {
            int characterClassID = int.Parse(keyValuePairs[PlayFabKeys.PlayerClass].Value);
            NetworkUtilities.setCustomProperty(PhotonNetwork.LocalPlayer, PlayerGlobalCustomProperties.PlayerClassID, characterClassID);
        }
    }
}