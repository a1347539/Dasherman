using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FYP.MainLobby
{
    public class RoomCardContainer : MonoBehaviour
    {
        [SerializeField]
        private RoomCard roomCard;

        private Transform container;

        private List<RoomCard> roomCards = new List<RoomCard>();
        // private List<RoomInfo> roomInfos = new List<RoomInfo>();

        private void Awake()
        {
            container = GetComponent<Transform>();
            RoomController.onOtherRoomCreated += handleRoomCreated;
            RoomController.onOtherRoomRemoved += handleRoomRemoved;
        }

        private void OnDestroy()
        {
            RoomController.onOtherRoomCreated -= handleRoomCreated;
            RoomController.onOtherRoomRemoved -= handleRoomRemoved;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void handleRoomCreated(RoomInfo info)
        {
            int index = roomCards.FindIndex(x => x.roomInfo.Name == info.Name);
            if (index == -1)
            {
                RoomCard card = Instantiate(roomCard, container);
                if (card != null)
                {
                    card.initializeRoomCard(info);
                    roomCards.Add(card);
                }
            }
            else
            {
                roomCards[index].updateRoomCard(info.PlayerCount);
            }
        }

        private void handleRoomRemoved(RoomInfo info)
        {
            int index = roomCards.FindIndex(x => x.roomInfo.Name == info.Name);
            if (index != -1)
            {
                Destroy(roomCards[index].gameObject);
                roomCards.RemoveAt(index);
            }
        }

    }
}