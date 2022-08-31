using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FYP.Global.InGame;
using System;

namespace FYP.InGame.UI.EndGameCanvas
{
    public class CountDownTimer : MonoBehaviour
    {
        public static Action onTimerDone = delegate { };

        [Tooltip("in second")]
        public float duration;

        public TMP_Text TimerText;

        private bool timerDone = false;

        void Update()
        {
            if (!timerDone)
            {
                duration -= Time.deltaTime;
                TimerText.text = $"{UIKeys.EndGameTimerMessage} {Mathf.FloorToInt(duration)}..";
                if (duration < 1)
                {
                    onTimerDone?.Invoke();
                    timerDone = true;
                }
            }
        }
    }
}