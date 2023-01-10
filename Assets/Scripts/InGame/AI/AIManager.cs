using FYP.InGame.AI.Agent;
using FYP.InGame.AI.Environment;
using FYP.InGame.AI.Environment.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;

namespace FYP.InGame.AI
{
    public class AIManager : Singleton<AIManager>
    {
        public float timerStart;
        private float timeLeft;

        public float durationInStep;

        private void Awake()
        {
            MapObjectManager.Instance.spawnCharacterAvailable += handleSpawnCharacters;
            MapObjectManager.Instance.spawnBreakableObjectAvailable += handleSpawnBreakableObject;
        }

        private void Start()
        {
            timeLeft = timerStart;
            durationInStep = timerStart / Time.fixedDeltaTime;
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                onTimerEnd();

            }
        }

        public void onTimerEnd() {
            for (int i = 0; i < MapObjectManager.Instance.characterContainer.childCount; ++i) {
                Destroy(MapObjectManager.Instance.characterContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < MapObjectManager.Instance.breakableObjectContainer.childCount; ++i)
            {
                Destroy(MapObjectManager.Instance.breakableObjectContainer.GetChild(i).gameObject);
            }

            MapController.Instance.createVirtualMatrix();


            handleSpawnCharacters();
            handleSpawnBreakableObject();


            timeLeft = timerStart;
        }



        private void handleSpawnCharacters()
        {
            MapObjectManager.Instance.spawnCharacter(1115, 1);
        }

        private void handleSpawnBreakableObject() {
            MapObjectManager.Instance.spawnBreakableObjects(15, 40);
        }
    }
}