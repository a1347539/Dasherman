using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

namespace FYP.MainLobby
{
    public class ConnectionController : MonoBehaviourPunCallbacks
    {
        // for debug only 
        /*        #region
                private void Awake()
                {
                    if (PhotonNetwork.NickName == String.Empty)
                    {
                        PhotonNetwork.NickName = Guid.NewGuid().ToString().Substring(0, 8);
                    }
                }

                void Start()
                {
                    if (PhotonNetwork.IsConnected) return;
                    PhotonNetwork.ConnectUsingSettings();
                }
        #endregion */


        public override void OnConnectedToMaster()
        {
            // print("Connected to server.");
            print("player name:" + PhotonNetwork.NickName);
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            print($"Disconnected from server: {cause}");
        }

        public override void OnJoinedLobby()
        {
            print("joined lobby");
        }
    }
}