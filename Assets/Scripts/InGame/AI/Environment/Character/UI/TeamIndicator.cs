using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.InGame.AI.Environment.Character
{
    public class TeamIndicator : MonoBehaviour
    {
        private Color selfColor = new Color(0, 0.67f, 0, 0.6f);
        private Color team1Color = new Color(0, 0f, 0.67f, 0.6f);
        private Color team2Color = new Color(0.67f, 0, 0, 0.6f);

        private void Start()
        {
            // GetComponent<Image>().color = selfColor;
        }

        public void setTeamColor(int teamNumber)
        {
            if (teamNumber == 1)
            {
                GetComponent<Image>().color = team1Color;
            }
            else
            {
                GetComponent<Image>().color = team2Color;
            }
        }
    }
}