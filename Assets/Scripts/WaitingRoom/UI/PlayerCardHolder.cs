using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;

namespace FYP.WaitingRoom
{
    public class PlayerCardHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject OccupiedUI;
        [SerializeField]
        private GameObject WaitingUI;
        [SerializeField]
        private GameObject LockedUI;

        [SerializeField]
        private PlayerCard playerCard;

        public PlayerCard Card { get { return playerCard; } }

        public int index;

        public static Action<int> onPlayerCardButtonClick = delegate { };

        private Action onStateChanged = delegate { };

        public enum States
        {
            Occupied, Waiting, Locked
        }

        private States state;

        public States State { 
            get { return state; }
            set { 
                state = value;
                switch (value)
                {
                    case States.Occupied:
                        OccupiedUI.SetActive(true);
                        WaitingUI.SetActive(false);
                        break;
                    case States.Waiting:
                        WaitingUI.SetActive(true);
                        OccupiedUI.SetActive(false);
                        break;
                    case States.Locked:
                        LockedUI.SetActive(true);
                        break;
                }
            }
        }

        public void initialize(int state, int index) {
            this.State = (States)state;
            this.index = index;
        }

        public void initializePlayerCard(Player player, Boolean isReady) {
            State = States.Occupied;
            playerCard.initialize(player, isReady);
        }

        public void toggleIsReady(Boolean isReady) {
            playerCard.toggleIsReady(isReady);
        }

        public void reset() {
            State = States.Waiting;
            playerCard.reset();
        }

        public void onClick() {
            onPlayerCardButtonClick?.Invoke(index);
        }
    }
}
