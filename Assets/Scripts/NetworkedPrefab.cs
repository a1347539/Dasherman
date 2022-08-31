using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FYP.Global {

    [Serializable]
    public class NetworkedPrefab
    {
        public GameObject prefab;
        public string path;

        public NetworkedPrefab(GameObject _prefab, string _path) {
            prefab = _prefab;
            path = _path;
        }
    }
}