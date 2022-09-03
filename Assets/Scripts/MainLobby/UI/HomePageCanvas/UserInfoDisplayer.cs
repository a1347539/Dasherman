using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using PlayFab.ClientModels;
using FYP.Global;
using System.Globalization;

namespace FYP.MainLobby
{
    public class UserInfoDisplayer : MonoBehaviour
    {
        [SerializeField]
        private Image classIcon;

        [SerializeField]
        private TMP_Text username;

        [SerializeField]
        private TMP_Text level;
        [SerializeField]
        private Slider ExpSlider;
        [SerializeField]
        private TMP_Text gold;

        private void Start()
        {
            NetworkManager.onGetPlayerClass += handleSetFields;
        }

        private void OnDestroy()
        {
            NetworkManager.onGetPlayerClass -= handleSetFields;
        }

        private void handleSetFields(Dictionary<string, UserDataRecord> keyValuePairs) {
            classIcon.sprite = MainLobbyManager.Instance.playerCharacter.classIcon;
            username.text = MainLobbyManager.Instance.playerUsername;

            level.text = $"LV. {keyValuePairs[PlayFabKeys.PlayerLevel].Value}";
            ExpSlider.value = int.Parse(keyValuePairs[PlayFabKeys.PlayerExp].Value) / GlobalMathFunctions.expByLevel(int.Parse(keyValuePairs[PlayFabKeys.PlayerLevel].Value));
            gold.text = int.Parse(keyValuePairs[PlayFabKeys.PlayerGold].Value).ToString("C0", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}