using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerInstance
{
    public class UIManager : MonoBehaviourPun
    {
        public static Action onSpriteEnabled = delegate { };

        [SerializeField]
        private GameObject sprite;
        [SerializeField]
        private GameObject indicatorCanvas;
        [SerializeField]
        private GameObject overlayCanvas;

        [SerializeField]
        private TeamIndicator teamIndicator;
        public TeamIndicator TeamIndicator { get { return teamIndicator; } }

        [SerializeField]
        private ParticleSystem teleportEffect;

        public void activateAllUI()
        {
            sprite.SetActive(true);
            onSpriteEnabled?.Invoke();
            indicatorCanvas.SetActive(true);
            overlayCanvas.SetActive(true);
            if (photonView.IsRoomView) return;
            UIController.Instance.joyStickCanvas.SetActive(true);
            UIController.Instance.itemWheelCanvas.SetActive(true);
        }

        public void deactivateAllUI() {
            sprite.SetActive(false);
            indicatorCanvas.SetActive(false);
            overlayCanvas.SetActive(false);
            if (photonView.IsRoomView) return;
            UIController.Instance.joyStickCanvas.SetActive(false);
            UIController.Instance.itemWheelCanvas.SetActive(false);
        }
    }
}