using FYP.InGame.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Global
{
    public class PersistentTeamManager : MonoBehaviour
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

        }
    }
}