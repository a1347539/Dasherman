using FYP.InGame.PlayerItemInstance;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace FYP.InGame.UI
{
    // at max 3 or 4 skills
    public class ItemWheel : MonoBehaviour
    {
        [SerializeField]
        private GameObject WheelSlotPrefab;

        public List<WheelSlot> wheelSlots { get; private set; }

        private Animator animator;
        private RectTransform rectTransform;
        private Vector2 uiOffset;

        private int numberOfSlots { get { return PlayerItemManager.Instance.numberOfSlots; } }

        private int selectedItemIndex = -1;

        private bool isInitialized = false;

        private void Awake()
        {
            setUI();
            if (InputManager.Instance.isTouchControl)
            {
                InputManager.onTouchDown += handleDisplayWheel;
                InputManager.onTouchMove += handleSelectItem;
                InputManager.onTouchUp += handleTouchUp;
            }
            else
            {
                InputManager.onMouseLeftButtonDown += handleDisplayWheelForMouseControl;
                InputManager.onMouseHoldDown += handleSelectItemForMouseControl;
                InputManager.onMouseLeftButtonUp += handleButtonUpForMouseControl;
                InputManager.onFKeyRelease += handleFKeyRelease;
            }
        }

        private void OnDestroy()
        {
            InputManager.onTouchDown -= handleDisplayWheel;
            InputManager.onTouchMove -= handleSelectItem;
            InputManager.onTouchUp -= handleTouchUp;

            InputManager.onMouseLeftButtonDown -= handleDisplayWheelForMouseControl;
            InputManager.onMouseHoldDown -= handleSelectItemForMouseControl;
            InputManager.onMouseLeftButtonUp -= handleButtonUpForMouseControl;
            InputManager.onFKeyRelease -= handleFKeyRelease;
        }

        void setUI()
        {
            animator = GetComponent<Animator>();
            rectTransform = GetComponent<RectTransform>();

            float radius = GetComponent<RectTransform>().sizeDelta.x/3;
            WheelSlot[] wss = new WheelSlot[numberOfSlots];

            for (int i = 0; i < numberOfSlots; ++i) {
                float radians = 2 * Mathf.PI / numberOfSlots * i;

                float vertical = -Mathf.Cos(radians);
                float horizontal = -Mathf.Sin(radians);

                Vector3 direction = new Vector2 (horizontal, vertical);
                Vector3 initPos = transform.position + direction * radius;
                GameObject slot = Instantiate(WheelSlotPrefab, initPos, Quaternion.identity, transform);
                wss[(i + 3) % 5] = slot.GetComponent<WheelSlot>();
            }
            wheelSlots = new List<WheelSlot>(wss);
        }

        public void initialize(List<int> amounts, List<GameObject> icons) {
            for (int i = 0; i < PlayerItemManager.Instance.numberOfSlots; ++i)
            {
                wheelSlots[i].initialize(i, amounts[i], icons[i]);
            }

            gameObject.SetActive(false);
            isInitialized = true;
        }

        public void disableSlot(int i, bool noMoreItem = false) {
            wheelSlots[i].Interactable = false;
            if (noMoreItem) { wheelSlots[i].hardLockSlot(); }
        }

        public void disableAllSlots() {
            for (int i = 0; i < PlayerItemManager.Instance.numberOfSlots; ++i) {
                disableSlot(i);
            }
        }

        public void enableSlot(int i)
        {
            wheelSlots[i].Interactable = true;
        }

        public void enableAllSlots() {
            for (int i = 0; i < PlayerItemManager.Instance.numberOfSlots; ++i)
            {
                enableSlot(i);
            }
        }

        private void handleDisplayWheel(Touch t) {
            if (!isInitialized) return;
            if (t.fingerId == 1) {
                selectedItemIndex = -1;
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, t.position);
                rectTransform.anchoredPosition = TouchInput.getScreenToWorldTouchPosition(screenPoint);
                gameObject.SetActive(true);
            }
        }

        private void handleSelectItem(Touch t)
        {
            if (!isInitialized) return;
            if (t.fingerId == 1) {
                TouchInput.TouchData td = TouchInput.Instance.touchDatas.First(td => td.touchID == t.fingerId);
                float distance = td.getDistanceFromOnTouch(t.position);
                if (distance < GetComponent<RectTransform>().sizeDelta.x / 7) {
                    if (selectedItemIndex != -1)
                    {
                        wheelSlots[selectedItemIndex].onDeselect();
                        selectedItemIndex = -1;
                    }
                    return;
                }
                float angle = td.getAngleFromOnTouch(t.position);

                int portion = 360 / numberOfSlots;
                int newIndex = Mathf.FloorToInt(Mathf.Abs(angle + portion - 450) / portion);
                if (newIndex == numberOfSlots) { --newIndex; }
                newIndex = (newIndex + 2) % numberOfSlots;
                if (selectedItemIndex != -1 && selectedItemIndex != newIndex) {
                    wheelSlots[selectedItemIndex].onDeselect();
                }
                selectedItemIndex = newIndex;
                wheelSlots[selectedItemIndex].onSelect();
            }
        }

        private void handleTouchUp(Touch t) {
            if (!isInitialized) return;
            if (selectedItemIndex != -1) {
                if (wheelSlots[selectedItemIndex].Interactable)
                {
                    PlayerItemManager.Instance.onItemSelected(selectedItemIndex);
                }
            }
            animator.SetTrigger("CloseWheel");
            selectedItemIndex = -1;
        }

        public void disableWheel() {
            gameObject.SetActive(false);
        }

        #region mouseControl 

        private void handleDisplayWheelForMouseControl(InputManager.MouseButtonData mouseButtonData)
        {
            if (!isInitialized) return;
            if (InputManager.isFKeyPressed && (
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.idle ||
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.useSkill
                )
                )
            {
                selectedItemIndex = -1;
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, mouseButtonData.onButtonDownPosition);
                rectTransform.anchoredPosition = TouchInput.getScreenToWorldTouchPosition(screenPoint);
                gameObject.SetActive(true);
            }
        }

        private void handleSelectItemForMouseControl(InputManager.MouseButtonData mouseButtonData, Vector2 currentPosition) {
            if (!isInitialized) return;
            if (InputManager.isFKeyPressed && (
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.idle ||
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.useSkill
                ))
            {
                float distance = mouseButtonData.getDistanceFromOnTouch(currentPosition);
                if (distance < GetComponent<RectTransform>().sizeDelta.x / 7)
                {
                    if (selectedItemIndex != -1)
                    {
                        wheelSlots[selectedItemIndex].onDeselect();
                        selectedItemIndex = -1;
                    }
                    return;
                }
                float angle = mouseButtonData.getAngleFromOnTouch(currentPosition);

                int portion = 360 / numberOfSlots;
                int newIndex = Mathf.FloorToInt(Mathf.Abs(angle + portion - 450) / portion);
                if (newIndex == numberOfSlots) { --newIndex; }
                newIndex = (newIndex + 2) % numberOfSlots;
                if (selectedItemIndex != -1 && selectedItemIndex != newIndex)
                {
                    wheelSlots[selectedItemIndex].onDeselect();
                }
                selectedItemIndex = newIndex;
                wheelSlots[selectedItemIndex].onSelect();
            }
            else {
                selectedItemIndex = -1;
            }
        }

        private void handleButtonUpForMouseControl(InputManager.MouseButtonData mouseButtonData, Vector2 pos) {
            if (!isInitialized) return;
            if (InputManager.isFKeyPressed && (
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.idle ||
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState == PlayerInstance.CharacterController.CharacterStates.useSkill
                ))
            {
                if (selectedItemIndex != -1)
                {
                    if (wheelSlots[selectedItemIndex].Interactable)
                    {
                        PlayerItemManager.Instance.onItemSelected(selectedItemIndex);
                    }
                }
                animator.SetTrigger("CloseWheel");
            }
            selectedItemIndex = -1;
        }

        private void handleFKeyRelease() {
            if (!isInitialized) return;
            if (selectedItemIndex != -1)
            {
                if (wheelSlots[selectedItemIndex].Interactable)
                {
                    PlayerItemManager.Instance.onItemSelected(selectedItemIndex);
                }
            }
            animator.SetTrigger("CloseWheel");
            selectedItemIndex = -1;
        }


        #endregion
    }
}