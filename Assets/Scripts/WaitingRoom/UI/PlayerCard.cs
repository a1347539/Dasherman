using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System;
using FYP.Global;
using System.Linq;
using FYP.InGame.PlayerInstance;

namespace FYP.WaitingRoom
{
    public class PlayerCard : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TMP_Text playerNameText;

        [SerializeField]
        private Transform playerCharacterPrefabTransform;

        [SerializeField]
        private Image isReadyIndicator;

        private GameObject currentObject;

        public Player player { get; private set; }

        private bool isReady = false;

        public bool IsReady { 
            get { return isReady; }
            set
            {
                isReady = value;
                isReadyIndicator.gameObject.SetActive(value);
            }
        }

        public void initialize(Player player, Boolean isReady) {
            int id = (int)NetworkUtilities.getCustomProperty(player, PlayerGlobalCustomProperties.PlayerClassID);
            ScriptableCharacter data = WaitingRoomManager.Instance.charactersDatas.First(data => data.characterId == id);
            currentObject = Instantiate(data.characterPrefab.transform.Find("Sprite"), 
                playerCharacterPrefabTransform.transform.position, 
                Quaternion.identity).gameObject;
            currentObject.transform.localScale = Vector2.one * 3;

            this.player = player;
            IsReady = isReady;
            playerNameText.text = player.NickName;
        }

        public void toggleIsReady(Boolean isReady) {
            IsReady = isReady;
        }

        public void reset() {
            Destroy(currentObject);
            playerNameText.text = string.Empty;
            player = null;
            isReady = false;
        }
    }
}