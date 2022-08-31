using FYP.InGame.PlayerItemInstance;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace FYP.InGame.UI
{
    [Serializable]
    public class WheelSlot : MonoBehaviour
    {
        private bool hardLocked = false;

        private bool interactable = true;
        public bool Interactable
        {
            get { return interactable; }
            set {
                if (!hardLocked)
                {
                    onChangeInteractable(value);
                }
            }
        }

        private int amountOfItem;

        [SerializeField]
        private GameObject disableMask;
        [SerializeField]
        public TMP_Text cooldownTimer;

        private Animator animator;

        public GameObject icon;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void initialize(int id, int amount, GameObject icon = null) {
            amountOfItem = amount;
            this.icon = icon;

            if (icon == null)
            {
                Interactable = false;
                hardLocked = true;
            }
            else
            {
                Instantiate(icon, transform).transform.SetSiblingIndex(1);
            }
        }

        public void decreaseItemAmount() {
            --amountOfItem;
            if (amountOfItem <= 0)
            {
                Interactable = false;
                hardLocked = true;
            }
        }

        public void hardLockSlot() {
            hardLocked = true;
        }

        public void onSelect() { 
            animator.SetBool("OnSelect", true);
        }

        public void onDeselect() {
            animator.SetBool("OnSelect", false);
        }

        private void onChangeInteractable(bool value) {
            disableMask.SetActive(!value);
            interactable = value;
        }


    }
}