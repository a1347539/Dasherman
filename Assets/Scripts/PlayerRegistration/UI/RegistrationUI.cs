using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FYP.InGame.PlayerInstance;
using FYP.Global.InGame;
using System;
using FYP.Global;

namespace FYP.PlayerRegistration
{
    public class RegistrationUI : Singleton<RegistrationUI>
    { 
        [SerializeField]
        private GameObject usernameForm;
        [SerializeField]
        private TMP_InputField usernameInputField;


        [SerializeField]
        private GameObject classSelection;

        private void Awake()
        {
            PlayerRegistrationManager.playerNotRegistered += handleStartRegistration;
        }

        private void OnDestroy()
        {
            PlayerRegistrationManager.playerNotRegistered -= handleStartRegistration;
        }

        private void handleStartRegistration()
        {
            usernameForm.SetActive(true);
        }

        public void onFillinUsernameButtonClick() {
            if (usernameInputField.text.Length < 3 || usernameInputField.text.Length > 12) return;
            PlayerRegistrationManager.Instance.username = usernameInputField.text;
            usernameForm.SetActive(false);
            classSelection.SetActive(true);
        }
    }
}