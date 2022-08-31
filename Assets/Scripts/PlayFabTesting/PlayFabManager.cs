using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace FYP.PlayFabTesting
{
    public class PlayFabManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            string customID = "123";
            login();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void login()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = "123",
                CreateAccount = false,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                    GetPlayerProfile = true,
                }
            };

            PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onError);

        }

        private void onSuccess(LoginResult result)
        {
            Debug.Log("Successful Login");
            string name = null;
            if (result.InfoResultPayload.PlayerProfile != null)
            {
                name = result.InfoResultPayload.PlayerProfile.DisplayName;
            }
        }

        private void onError(PlayFabError obj)
        {
            Debug.Log($"Unseccessful Login: {obj.GenerateErrorReport()}");
        }
    }
}