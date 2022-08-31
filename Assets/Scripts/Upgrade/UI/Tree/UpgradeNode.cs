using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FYP.Upgrade
{
    public class UpgradeNode : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Sprite[] sprites;

        private void Start()
        {
            
        }

        public void initialize(int id) {
            text.text = id.ToString();
            background.sprite = sprites[id/100];
        }
    }
}