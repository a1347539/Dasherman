using FYP.Global.InGame;
using FYP.InGame.PlayerInstance;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.WaitingRoom
{
    public class WaitingRoomManager : Singleton<WaitingRoomManager>
    {
        public static Action onDataReady = delegate { };

        public ScriptableCharacter[] charactersDatas { get; private set; }


        private void Start()
        {
            charactersDatas = Resources.LoadAll<ScriptableCharacter>(PlayerKeys.scriptableCharacterPathPrefix);
            onDataReady?.Invoke();
        }
    }
}