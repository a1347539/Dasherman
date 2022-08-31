using FYP.InGame.PlayerInstance;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.InGame.UI {
    public class StatusBars : MonoBehaviourPun
    {
        [SerializeField]
        private Slider healthBar;
        [SerializeField]
        private Slider ManaBar;
        [SerializeField]
        private CharacterVital vital;

        private void Awake()
        {
            if (!photonView.IsMine) return;
            vital.onSetHealth += handleSetHealth;
            vital.onSetMana += handleSetMana;
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            vital.onSetHealth -= handleSetHealth;
            vital.onSetMana -= handleSetMana;
        }


        // all values are normalized to 0 - 1

        private void handleSetHealth(float health)
        {
            healthBar.value = health;
        }

        private void handleSetMana(float mana)
        {
            ManaBar.value = mana;
        }
    }
}