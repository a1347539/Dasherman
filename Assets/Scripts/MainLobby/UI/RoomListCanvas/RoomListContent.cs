using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace FYP.MainLobby
{
    public class RoomListContent : MonoBehaviour
    {
        [SerializeField]
        private GameObject UIMask;

        [SerializeField]
        private Button backButton;

        private void Awake()
        {
            HomePageContent.onOpenRoomListButtonClick += handleOpenRoomListContent;
        }

        private void Start()
        {
            backButton.onClick.AddListener(handleCloseRoomList);
        }

        private void OnDestroy()
        {
            HomePageContent.onOpenRoomListButtonClick -= handleOpenRoomListContent;
            backButton.onClick.RemoveAllListeners();
        }

        private void handleOpenRoomListContent()
        {
            UIMask.SetActive(true);
            GetComponent<Animator>().SetBool("open", true);
        }

        private void handleCloseRoomList() { 
            GetComponent<Animator>().SetBool("open", false);
            StartCoroutine(waitForAnimation());

            IEnumerator waitForAnimation() {
                yield return new WaitForSeconds(0.5f);
                UIMask.SetActive(false);
            }
        }
    }
}