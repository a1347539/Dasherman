using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using FYP.Global;
using System;

namespace FYP.PlayerRegistration
{
    public class PlayFabRegistrationManager : MonoBehaviour
    {
        private void Awake()
        {
            ClassSelectionUI.onClassConfirmSelection += handleClassConfirmSelection;
        }

        private void OnDestroy()
        {
            ClassSelectionUI.onClassConfirmSelection -= handleClassConfirmSelection;
        }

        private void handleClassConfirmSelection(int classID)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = PlayerRegistrationManager.Instance.username,
            },
            (result) => { print($"username changed to {result.DisplayName}"); },
            onError
            );

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> {
                    { PlayFabKeys.PlayerClass, classID.ToString() }
                }
            },
            (result) => { 
                print($"user choose {classID}");
                PlayerRegistrationManager.playerRegistered?.Invoke(PlayerRegistrationManager.Instance.username);
            },
            onError
            );
        }

        private void onError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }
    }
}