using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using FYP.Global;

namespace FPY.Login
{
    public class PlayFabConnectionController : Singleton<PlayFabConnectionController>
    {
        private void Awake()
        {
            LoginManager.onLoginButtonClicked += playFabLogin;
        }

        private void OnDestroy()
        {
            LoginManager.onLoginButtonClicked -= playFabLogin;
        }

        void Start()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.TitleId)) {
                PlayFabSettings.TitleId = "3B9B2";
            }
        }

        private void playFabLogin(string username)
        {
            var request = new LoginWithCustomIDRequest {
                CustomId = username,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
            };
            PlayFabClientAPI.LoginWithCustomID(request, 
                (result) => {
                    string username;
                    if (result.InfoResultPayload.PlayerProfile == null)
                    {
                        username = string.Empty;
                    }
                    else {
                        username = result.InfoResultPayload.PlayerProfile.DisplayName;
                    }
                    print(username);
                    print($"Successfully Login/created account {username}");
                    PhotonConnectionController.Instance.connectToPhoton(username);
                }, 
                onError
            );
        }

        private void onError(PlayFabError error)
        {
            print($"Unsuccessful: {error.GenerateErrorReport()}");
        }
    }
}