using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FYP.WaitingRoom
{
    public class StartGameButton : MonoBehaviour
    {
        public static Action onStartGameButtonClick = delegate { };

        public void onClick()
        {
            onStartGameButtonClick?.Invoke();
        }


    }
}