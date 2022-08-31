using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TouchInput : Singleton<TouchInput>
{
    [Serializable]
    public struct TouchData {
        public int touchID;
        public Vector2 onTouchDownPosition;
        public TouchData(int id, Vector2 pos) { touchID = id; onTouchDownPosition = pos; }
        public float getAngleFromOnTouch(Vector2 currentPos) {
            Vector2 normalizedPosition = currentPos - onTouchDownPosition;
            if (normalizedPosition != Vector2.zero)
            {
                float angleFromTouchToCurrent = (float)((Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) / Mathf.PI) * 180f);
                if (angleFromTouchToCurrent < 0) angleFromTouchToCurrent += 360f;
                return angleFromTouchToCurrent;
            }
            return -1;
        }
        public float getDistanceFromOnTouch(Vector2 currentPos) { 
            return Vector2.Distance(onTouchDownPosition, currentPos);
        }
    }

    public static Action<Touch> onTouchDown = delegate { };
    public static Action<Touch> onTouchMove = delegate { };
    public static Action<Touch> onTouchUp = delegate { };

    public List<TouchData> touchDatas { get; private set; } = new List<TouchData>();

    public static Vector2 getScreenToWorldTouchPosition(Vector2 touchPosition)
    {
        return Camera.main.ScreenToWorldPoint(touchPosition);
    }

    private void Update()
    {
        for (int i = 0; i < Input.touchCount; ++i) { 
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                touchDown(t);
            }
            else if (t.phase == TouchPhase.Moved)
            {
                touchMove(t);
            }
            else if (t.phase == TouchPhase.Ended)
            {
                touchUp(t);
            }
        }
    }

    private void touchDown(Touch touch) {
        touchDatas.Add(new TouchData(touch.fingerId, touch.position));
        onTouchDown?.Invoke(touch);
    }

    private void touchMove(Touch touch) {
        onTouchMove?.Invoke(touch);
    }

    private void touchUp(Touch touch) {
        TouchData t =  touchDatas.First(t => t.touchID == touch.fingerId);
        touchDatas.Remove(t);
        onTouchUp?.Invoke(touch);
    }
}
