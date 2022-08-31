using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

namespace FYP.MainLobby
{
    public class UserInfoDisplayer : MonoBehaviour
    {
        [SerializeField]
        private Image classIcon;

        [SerializeField]
        private TMP_Text username;

        private void Start()
        {
            NetworkManager.onGetPlayerClass += handleSetFields;
        }

        private void OnDestroy()
        {
            NetworkManager.onGetPlayerClass -= handleSetFields;
        }

        private void handleSetFields(int id) {
            classIcon.sprite = MainLobbyManager.Instance.playerCharacter.classIcon;
            username.text = MainLobbyManager.Instance.playerUsername;


        }
    }
}