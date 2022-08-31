using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FYP.InGame.PlayerInstance
{
    public class MyFloatingJoystick : Joystick
    {
        protected override void Start()
        {
            base.Start();
            background.gameObject.SetActive(false);
            if (InputManager.Instance.isTouchControl)
            {
                InputManager.onTouchDown += handleTouchDown;
                InputManager.onTouchMove += handleTouchMove;
                InputManager.onTouchUp += handleTouchUp;
            }
        }

        private void OnDestroy()
        {
            InputManager.onTouchDown -= handleTouchDown;
            InputManager.onTouchMove -= handleTouchMove;
            InputManager.onTouchUp -= handleTouchUp;
        }

        private void handleTouchDown(Touch t)
        {
            if (t.fingerId == 0)
            {
                Vector2 onPointerDownPosition = new Vector2(t.position.x, t.position.y);
                background.anchoredPosition = ScreenPointToAnchoredPosition(onPointerDownPosition);
                background.gameObject.SetActive(true);
            }
        }

        private void handleTouchMove(Touch t)
        {
            if (t.fingerId == 0)
            {
                Vector2 onMovePosition = new Vector2(t.position.x, t.position.y);
                onTouchInputMove(onMovePosition);
            }
        }

        private void handleTouchUp(Touch t)
        {
            if (t.fingerId == 0)
            {
                background.gameObject.SetActive(false);
                onTouchInputUp();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (InputManager.Instance.isTouchControl) return;
            if (InputManager.isFKeyPressed) return;
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (InputManager.Instance.isTouchControl) return;
            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }
}