using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FYP.PlayerRegistration
{
    public class PhotonRegistrationManager : MonoBehaviour
    {

        private void Awake()
        {
            PlayerRegistrationManager.onSceneLoaded += handleSceneLoaded;
        }

        private void OnDestroy()
        {
            PlayerRegistrationManager.onSceneLoaded -= handleSceneLoaded;
        }

        private void handleSceneLoaded() {
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PlayerRegistrationManager.playerNotRegistered?.Invoke();
            }
            else {
                PlayerRegistrationManager.playerRegistered?.Invoke(PhotonNetwork.NickName);
            }
        
        }
    }
}