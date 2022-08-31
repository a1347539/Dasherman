using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FYP.InGame.UI.EndGameCanvas
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private TMP_Text EXPText;
        [SerializeField]
        private TMP_Text GoldText;

        public void initialize(int level, string name, int score, int EXP, int gold, bool isMine)
        {
            levelText.text = $"Lv.{level}";
            nameText.text = name;
            scoreText.text = score.ToString();
            EXPText.text = EXP.ToString();
            GoldText.text = $"${gold}";
        }
    }
}