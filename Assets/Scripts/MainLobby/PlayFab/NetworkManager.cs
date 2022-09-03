using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using FYP.Global;
using System;

namespace FYP.MainLobby
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public static Action<Dictionary<string, UserDataRecord>> onGetPlayerClass = delegate { };

        private void Start()
        {
            getPlayerData();
        }

        private void getPlayerData() {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                Keys = new List<string> { 
                    PlayFabKeys.PlayerClass,
                    PlayFabKeys.PlayerLevel,
                    PlayFabKeys.PlayerGold,
                    PlayFabKeys.PlayerExp,
                }
            },
            (result) =>
            {
                if (result.Data != null)
                {
                    onGetPlayerClass?.Invoke(result.Data);
                }
            },
            onError
            );
        }

        private void onError(PlayFabError obj)
        {
            print(obj.GenerateErrorReport());
        }
    }
}