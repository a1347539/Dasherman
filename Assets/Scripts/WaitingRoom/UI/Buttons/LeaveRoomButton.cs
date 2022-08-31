using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.WaitingRoom
{
    public class LeaveRoomButton : MonoBehaviour
    {
        public static Action onLeaveRoomButtonClick = delegate { };

        public void onClick() {
            print("clicked");
            onLeaveRoomButtonClick?.Invoke();
            // MainMenus.UIController.Instance.gotoMainLobby();
        }
    }
}