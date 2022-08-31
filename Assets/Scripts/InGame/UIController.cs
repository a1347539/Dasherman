using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FYP.InGame
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField]
        private GameObject playerDeadCanvas;
        [SerializeField]
        private GameObject endGameCanvas;

        [SerializeField]
        public GameObject joyStickCanvas;
        public GameObject itemWheelCanvas;


        private void Awake()
        {
            GameManager.onSelfDie += handleSelfDie;
            GameManager.onEndGame += handleEndGame;
        }

        private void OnDestroy()
        {
            GameManager.onSelfDie -= handleSelfDie;
            GameManager.onEndGame -= handleEndGame;
        }

        private void handleSelfDie()
        {
            playerDeadCanvas.SetActive(true);
        }

        private void handleEndGame() {
            StartCoroutine(displayEndGameCanvas());
        }

        IEnumerator displayEndGameCanvas() {
            yield return new WaitForSeconds(playerDeadCanvas.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);
            endGameCanvas.SetActive(true);
        }
    }
}