using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.InGame.PlayerInstance
{
    public class TeamIndicator : MonoBehaviourPun
    {
        private Color selfColor = new Color(0, 0.67f, 0, 0.6f);
        private Color teammateColor = new Color(0, 0f, 0.67f, 0.6f);
        private Color enemyColor = new Color(0.67f, 0, 0, 0.6f);

        private void Start()
        {
            if (!photonView.IsMine) return;
            GetComponent<Image>().color = selfColor;
        }

        public void setTeamColor(bool isSameTeam)
        {
            if (isSameTeam)
            {
                GetComponent<Image>().color = teammateColor;
            }
            else
            {
                GetComponent<Image>().color = enemyColor;
            }
        }
    }
}