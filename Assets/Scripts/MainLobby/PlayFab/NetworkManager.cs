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
        public static Action<int> onGetPlayerClass = delegate { };

        private void Start()
        {
            getPlayerClass();
        }

        private void getPlayerClass() {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                Keys = new List<string> { PlayFabKeys.PlayerClass }
            },
            (result) =>
            {
                if (result.Data != null && result.Data.ContainsKey(PlayFabKeys.PlayerClass))
                {
                    onGetPlayerClass?.Invoke(Int32.Parse(result.Data[PlayFabKeys.PlayerClass].Value));
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