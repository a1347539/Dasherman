using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FYP.InGame.Photon;
using FYP.Global.InGame;
using FYP.Global.Photon;
using FYP.Global;

namespace FYP.InGame.UI.EndGameCanvas
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField]
        private Transform[] playerCardContainers;

        [SerializeField]
        private PlayerCard playerCardPrefab;
        [SerializeField]
        private GameObject titleCardPrefab;

        // Start is called before the first frame update
        void Start()
        {
            int i = 0;
            foreach (PhotonTeam team in PhotonTeamsManager.Instance.PhotonTeams) {
                Player[] players;
                PhotonTeamsManager.Instance.TryGetTeamMembers(team.Code, out players);
                foreach (Player player in players) {
                    Instantiate(playerCardPrefab.gameObject, playerCardContainers[i % 2]).GetComponent<PlayerCard>().initialize(
                    1,
                    player.NickName,
                    (int)NetworkUtilities.getCustomProperty(player, PlayerKeys.InGameScore),
                    0,
                    0,
                    player == PhotonNetwork.LocalPlayer
                    );
                }
                ++i;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}