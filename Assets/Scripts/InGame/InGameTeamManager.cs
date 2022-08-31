using FYP.Global.InGame;
using FYP.Global.Photon;
using FYP.InGame.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FYP.InGame
{
    public class InGameTeam {
        public byte code;
        public List<PhotonView> views;
        public override string ToString()
        {
            return $"{code}, {views.Count}";
        }
    }

    public class InGameTeamManager : PunSingleton<InGameTeamManager>
    {
        public Dictionary<byte, InGameTeam> byteInGameTeamsPair = new Dictionary<byte, InGameTeam>();

        private void Awake()
        {
            RoomController.onPrejoinGame += handleCreateInGameTeams;
        }

        private void OnDestroy()
        {
            RoomController.onPrejoinGame -= handleCreateInGameTeams;
        }

        private void handleCreateInGameTeams()
        {
            foreach (PhotonTeam pteam in PhotonTeamsManager.Instance.PhotonTeams) {
                if (pteam.Code == 250) continue;
                byteInGameTeamsPair.Add(pteam.Code, new InGameTeam
                {
                    code = pteam.Code,
                    views = new List<PhotonView>(),
                });
            }
            byteInGameTeamsPair.Add((byte)AIKeys.AITeamNumber, new InGameTeam
            {
                code = (byte)AIKeys.AITeamNumber,
                views = new List<PhotonView>(),
            });
        }

        public InGameTeam getTeamByCode(byte code) {
            return byteInGameTeamsPair[code];
        }

        public InGameTeam getTeamByPhotonView(PhotonView pv) {
            return byteInGameTeamsPair.Values.First(team => team.views.Any(view => view.ViewID == pv.ViewID));
        }

        public List<InGameTeam> getAllTeams() {
            return byteInGameTeamsPair.Values.ToList();
        }

        public void joinTeam(byte code, PhotonView view) {
            print($"adding {view.ViewID} to team {code}");
            byteInGameTeamsPair[code].views.Add(view);
        }

        public bool isInTeam(PhotonView pv) {
            return byteInGameTeamsPair.Values.Any(team => team.views.Contains(pv));
        }

        public void onInGameTeamDebugButtonClick() {
            foreach (KeyValuePair<byte, InGameTeam> item in byteInGameTeamsPair) {
                print($"code: {item.Key} internal Code: {item.Value.code}");
                print("photonView: " + string.Join(", ", item.Value.views.ConvertAll(v => v.ViewID)));
            }
        }
    }
}