using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Linq;
using FYP.InGame.Photon;
using static FYP.Global.InputUtilities;

namespace FYP.InGame
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField]
        public bool isTouchControl = false;

        // touch control
        #region touch control

        public static Action<Touch> onTouchDown = delegate { };
        public static Action<Touch> onTouchMove = delegate { };
        public static Action<Touch> onTouchUp = delegate { };

        public static List<TouchData> touchDatas { get; private set; } = new List<TouchData>();

        #endregion

        // mouse control
        public static Action<MouseButtonData> onMouseLeftButtonDown = delegate { };
        public static Action<MouseButtonData, Vector2> onMouseLeftButtonUp = delegate { };
        public static Action<MouseButtonData, Vector2> onMouseHoldDown = delegate { };
        public static Action onFKeyRelease = delegate { };

        public static MouseButtonData mouseButtonData;

        public static bool isFKeyPressed { get; private set; }


        public static Vector2 getScreenToWorldTouchPosition(Vector2 touchPosition)
        {
            return Camera.main.ScreenToWorldPoint(touchPosition);
        }

        private void Update()
        {
            if (!RoomController.roomJoined) return;
            if (isTouchControl)
            {
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.phase == TouchPhase.Began)
                    {
                        touchDown(t);
                    }
                    else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                    {
                        touchMove(t);
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        touchUp(t);
                    }
                }
            }
            else {
                if (Input.GetKeyDown("f"))
                {
                    isFKeyPressed = true;
                }
                else if (Input.GetKeyUp("f"))
                {
                    isFKeyPressed = false;
                    onFKeyRelease?.Invoke();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    mouseButtonData = new MouseButtonData(Input.mousePosition);
                    onMouseLeftButtonDown?.Invoke(mouseButtonData);
                }

                else if (Input.GetMouseButtonUp(0))
                {
                    onMouseLeftButtonUp?.Invoke(mouseButtonData, Input.mousePosition);
                }

                else if (Input.GetMouseButton(0))
                {
                    onMouseHoldDown?.Invoke(mouseButtonData, Input.mousePosition);
                }
               
            }
        }

        private void touchDown(Touch touch)
        {
            touchDatas.Add(new TouchData(touch.fingerId, touch.position));
            //onTouchDown?.Invoke(touch);
        }

        private void touchMove(Touch touch)
        {
            //onTouchMove?.Invoke(touch);
        }

        private void touchUp(Touch touch)
        {
            TouchData t = touchDatas.First(t => t.touchID == touch.fingerId);
            touchDatas.Remove(t);
            //onTouchUp?.Invoke(touch);
        }
    }
}