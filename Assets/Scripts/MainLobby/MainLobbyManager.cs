using FYP.Global.InGame;
using FYP.InGame.PlayerInstance;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace FYP.MainLobby
{
    public class MainLobbyManager : Singleton<MainLobbyManager>
    {
        public ScriptableCharacter[] charactersDatas { get; private set; }

        public ScriptableCharacter playerCharacter { get; private set; }
        public string playerUsername { get; private set; }

        private void Start()
        {
            charactersDatas = Resources.LoadAll<ScriptableCharacter>(PlayerKeys.scriptableCharacterPathPrefix);
            playerUsername = PhotonNetwork.NickName;

            NetworkManager.onGetPlayerClass += handleGetPlayerClass;
        }

        private void OnDestroy()
        {
            NetworkManager.onGetPlayerClass -= handleGetPlayerClass;
        }

        private void handleGetPlayerClass(int characterClassID) {
            playerCharacter = charactersDatas.First(data => data.characterId == characterClassID);
        }
    }
}