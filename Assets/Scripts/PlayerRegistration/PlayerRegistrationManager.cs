using FYP.Global;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FYP.PlayerRegistration
{
    public class PlayerRegistrationManager : Singleton<PlayerRegistrationManager>
    {

        public static Action<string> playerRegistered = delegate { };
        public static Action playerNotRegistered = delegate { };

        public static Action onSceneLoaded = delegate { };

        public string username;

        private void Awake()
        {
            playerRegistered += handleChangeScene;
        }

        private void OnDestroy()
        {
            playerRegistered -= handleChangeScene;
        }

        private void Start()
        {
            onSceneLoaded?.Invoke();
        }

        private void handleChangeScene(string username) {
            PhotonNetwork.NickName = username;
            SceneController.LoadScene(SceneName.PlayerRegistration, SceneName.MainLobby);
        }
    }
}