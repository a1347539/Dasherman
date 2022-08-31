using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Linq;
using FYP.InGame.Photon;

namespace FYP.InGame
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField]
        public bool isTouchControl = false;

        // touch control
        #region touch control
        public struct TouchData
        {
            public int touchID;
            public Vector2 onTouchDownPosition;
            public Vector2 onTouchDownWorldPosition { get { return Camera.main.ScreenToWorldPoint(onTouchDownPosition); } }
            public TouchData(int id, Vector2 pos) { touchID = id; onTouchDownPosition = pos; }
            public float getAngleFromOnTouch(Vector2 currentPos)
            {
                Vector2 normalizedPosition = currentPos - onTouchDownPosition;
                if (normalizedPosition != Vector2.zero)
                {
                    float angleFromTouchToCurrent = (float)((Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) / Mathf.PI) * 180f);
                    if (angleFromTouchToCurrent < 0) angleFromTouchToCurrent += 360f;
                    return angleFromTouchToCurrent;
                }
                return -1;
            }
            public float getDistanceFromOnTouch(Vector2 currentPos)
            {
                return Vector2.Distance(onTouchDownPosition, currentPos);
            }
            public Vector2 getDeltaFromCurrentToOrigin(Vector2 current) { 
                return new Vector2(current.x - onTouchDownPosition.x, current.y - onTouchDownPosition.y);
            }
            public Vector2 getDeltaFromCurrentToOriginInWorldSpace(Vector2 current) {
                return current - onTouchDownWorldPosition;
            }
        }

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

        public struct MouseButtonData {
            public Vector2 onButtonDownPosition;
            public Vector2 onButtonDownWorldPosition { get { return Camera.main.ScreenToWorldPoint(onButtonDownPosition); } }
            public MouseButtonData(Vector2 pos) { onButtonDownPosition = pos; }
            public float getAngleFromOnTouch(Vector2 currentPos)
            {
                Vector2 normalizedPosition = currentPos - onButtonDownPosition;
                if (normalizedPosition != Vector2.zero)
                {
                    float angleFromTouchToCurrent = (float)((Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) / Mathf.PI) * 180f);
                    if (angleFromTouchToCurrent < 0) angleFromTouchToCurrent += 360f;
                    return angleFromTouchToCurrent;
                }
                return -1;
            }
            public float getDistanceFromOnTouch(Vector2 currentPos)
            {
                return Vector2.Distance(onButtonDownPosition, currentPos);
            }
            public Vector2 getDeltaFromCurrentToOrigin(Vector2 current)
            {
                return new Vector2(current.x - onButtonDownPosition.x, current.y - onButtonDownPosition.y);
            }
            public Vector2 getDeltaFromCurrentToOriginInWorldSpace(Vector2 current)
            {
                return current - onButtonDownWorldPosition;
            }
        }

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