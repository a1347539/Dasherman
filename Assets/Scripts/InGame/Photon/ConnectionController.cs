using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global.Photon;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using FYP.Global;
using FYP.Global.InGame;

namespace FYP.InGame.Photon
{
    public class ConnectionController : MonoBehaviourPunCallbacks
    {

        private const string debugRoomName = "debugRoom";

        public static Action onJoinRoom = delegate { };

        public static Action onPlayerDebugItemLoaded = delegate { };

        [SerializeField]
        private GameObject ptm;

        private void Awake()
        {
            if (PhotonNetwork.NickName == String.Empty)
            {
                PhotonNetwork.NickName = Guid.NewGuid().ToString().Substring(0, 8);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected) return;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            // print("Connected to server.");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedLobby()
        {
            print("player name:" + PhotonNetwork.NickName);
            if (!PhotonNetwork.InRoom)
            {
                PhotonNetwork.JoinOrCreateRoom(debugRoomName, new RoomOptions(), TypedLobby.Default);
            }
        }

        public override void OnJoinedRoom()
        {
            ptm.SetActive(true);
            ptm.GetComponent<PhotonTeamsManager>().PhotonTeams = createTeams();
            
            PhotonNetwork.LocalPlayer.JoinTeam(ptm.GetComponent<PhotonTeamsManager>().PhotonTeams[0].Code);
            onJoinRoom?.Invoke();
        }

        private List<PhotonTeam> createTeams()
        {
            List<PhotonTeam> photonTeams = new List<PhotonTeam>();

            for (int i = 0; i < 2; ++i)
            {
                photonTeams.Add(new PhotonTeam
                {
                    Name = $"Team {i + 1}",
                    Code = (byte)(i + 1),
                });
            }
            return photonTeams;
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
        }
    }
}