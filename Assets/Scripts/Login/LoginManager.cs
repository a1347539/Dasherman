using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global;
using TMPro;
using System;

namespace FPY.Login
{
    public class LoginManager : Singleton<LoginManager>
    {
        [SerializeField]
        private TMP_InputField usernameInput;

        public static Action<String> onLoginButtonClicked = delegate { };

        public void onLoginButtonClick()
        {
            //PlayFabConnectionController.Instance.playFabLogin(usernameInput.text);
            onLoginButtonClicked?.Invoke(usernameInput.text);

        }

        public void changeScene() {
            SceneController.LoadScene(SceneName.Login, SceneName.PlayerRegistration);
        }
    }
}