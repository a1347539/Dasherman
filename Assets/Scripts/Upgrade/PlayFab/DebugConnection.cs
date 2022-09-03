using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Upgrade
{
    public class DebugConnection : MonoBehaviour
    {

        public static Action onConnectedToPlayFab = delegate { };

        void Start()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = "3B9B2";
            }
            playFabLogin("123");
        }

        private void playFabLogin(string username)
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = username,
                CreateAccount = false,
            };
            PlayFabClientAPI.LoginWithCustomID(request,
                (result) => {
                    print("connected"); onConnectedToPlayFab?.Invoke();
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