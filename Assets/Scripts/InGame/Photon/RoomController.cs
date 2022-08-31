using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using ExitGames.Client.Photon;
using FYP.Global.InGame;
using Photon.Realtime;
using FYP.InGame.Map;
using FYP.Global;

namespace FYP.InGame.Photon {
    public class RoomController : MonoBehaviourPunCallbacks
    {
        public static bool roomJoined = false;

        public static Action onPrejoinGame = delegate { };
        public static Action onNormalJoin = delegate { };
        public static Action onDebugJoin = delegate { };
        public static Action onJoinGame = delegate { };
        public static Action onOtherPlayerLeft = delegate { };
        

        private void Awake()
        {
            //SceneManager.sceneLoaded += handleJoinedGame;
            ConnectionController.onJoinRoom += handleJoinedGame;
        }

        private void OnDestroy()
        {
            //SceneManager.sceneLoaded -= handleJoinedGame;
            ConnectionController.onJoinRoom -= handleJoinedGame;
        }

        private void handleJoinedGame(Scene arg0, LoadSceneMode arg1)
        {
            registerCustomType();
            onPrejoinGame?.Invoke();
            onNormalJoin?.Invoke();
            onJoinGame?.Invoke();
            roomJoined = true;
        }

        private void handleJoinedGame()
        {
            registerCustomType();
            onPrejoinGame?.Invoke();
            onDebugJoin?.Invoke();
            onJoinGame?.Invoke();
            roomJoined = true;
        }

        private void registerCustomType() {
            PhotonPeer.RegisterType(typeof(Point), PhotonCodes.pointType, Point.serialize, Point.deserialize);
        }

        public override void OnLeftRoom()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                // need to transfer all NPC's ownership to other

                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer.GetNext());
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                onOtherPlayerLeft?.Invoke();
            }
        }
    }
}