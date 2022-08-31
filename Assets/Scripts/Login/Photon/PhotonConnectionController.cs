using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global;
using Photon.Pun;

namespace FPY.Login
{
    public class PhotonConnectionController : Singleton<PhotonConnectionController>
    {
        public void connectToPhoton(string username) {
            PhotonNetwork.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
            LoginManager.Instance.changeScene();
        }
    }
}
