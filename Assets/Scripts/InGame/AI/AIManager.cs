using FYP.InGame.AI.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] MapObjectManager mapObjectManager;

        private void Awake()
        {
            MapObjectManager.Instance.spawnCharacterAvailable += handleSpawnCharacters;
        }

        private void handleSpawnCharacters()
        {
            MapObjectManager.Instance.spawnCharacter(1115, 1);
        }
    }
}