using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.WaitingRoom {
    public class PersistentObject : MonoBehaviour
    {
        private static GameObject instance;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (instance == null)
            {
                instance = gameObject;
            }
            else
            {
                Destroy(gameObject);
            }

            RoomController.onLeaveRoom += handleDestroySelf;
        }

        private void OnDestroy()
        {
            RoomController.onLeaveRoom -= handleDestroySelf;
        }

        private void handleDestroySelf() {
            Destroy(gameObject);
        }
    }
}