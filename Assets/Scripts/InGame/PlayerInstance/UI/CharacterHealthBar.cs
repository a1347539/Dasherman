using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterHealthBar : MonoBehaviourPun
    {
        [SerializeField]
        private Slider healthBar;

        private void Awake()
        {
            if (!photonView.IsMine) return;
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
        }

        // all values are normalized to 0 - 1


        public void setHealth(float health)
        {
            healthBar.value = health;
        }
    }
}