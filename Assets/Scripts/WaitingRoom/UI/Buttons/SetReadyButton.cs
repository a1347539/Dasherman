using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace FYP.WaitingRoom { 
    public class SetReadyButton : MonoBehaviour
    {
        public static Action<Player> onReady = delegate { };

        public void onClick() {
            onReady?.Invoke(PhotonNetwork.LocalPlayer);
        }
    }
}