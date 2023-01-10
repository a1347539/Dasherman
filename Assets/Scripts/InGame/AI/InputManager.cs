using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FYP.Global.InputUtilities;

namespace FYP.InGame.AI
{
    public class InputManager : MonoBehaviour
    {

        // mouse control
        public static Action<MouseButtonData> onMouseLeftButtonDown = delegate { };
        public static Action<int> onMouseLeftButtonUp = delegate { };
        public static Action<MouseButtonData, Vector2> onMouseHoldDown = delegate { };
        public static Action onFKeyRelease = delegate { };

        public static MouseButtonData mouseButtonData;

        public static bool isFKeyPressed { get; private set; } = false;


        public static Vector2 getScreenToWorldTouchPosition(Vector2 touchPosition)
        {
            return Camera.main.ScreenToWorldPoint(touchPosition);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseButtonData = new MouseButtonData(Input.mousePosition);
                onMouseLeftButtonDown?.Invoke(mouseButtonData);
            }

            else if (Input.GetMouseButtonUp(0))
            {
                onMouseLeftButtonUp?.Invoke(-1);
            }

            else if (Input.GetMouseButton(0))
            {
                onMouseHoldDown?.Invoke(mouseButtonData, Input.mousePosition);
            }
        }
    }
}