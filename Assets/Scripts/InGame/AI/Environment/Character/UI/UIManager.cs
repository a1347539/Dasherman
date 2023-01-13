using FYP.InGame.PlayerInstance;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Character
{
    public class UIManager : MonoBehaviour
    {
        public static Action onSpriteEnabled = delegate { };

        [SerializeField]
        private GameObject sprite;

        [SerializeField]
        private GameObject indicatorCanvas;

        [SerializeField]
        private TeamIndicator teamIndicator;

        [SerializeField]
        private GameObject overlayCanvas;

        public TeamIndicator TeamIndicator { get { return teamIndicator; } }

        public void activateAllUI()
        {
            sprite.SetActive(true);
            indicatorCanvas.SetActive(true);
            overlayCanvas.SetActive(true);
            onSpriteEnabled?.Invoke();
        }

        public void deactivateAllUI()
        {
            sprite.SetActive(false);
            indicatorCanvas.SetActive(false);
            overlayCanvas.SetActive(false);
        }
    }
}