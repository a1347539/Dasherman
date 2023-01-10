using FYP.InGame.AI.Environment;
using System;
using UnityEngine;
using FYP.InGame.AI;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;
using FYP.InGame.AI.Agent;

namespace FYP.InGame.AI
{
    public class AIManager : Singleton<AIManager>
    {
        public Action onTimerEnd = delegate { };

        public float timerStart;
        private float timeLeft;

        public float durationInStep;

        GameObject agent1;

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
                onTimerEnd?.Invoke();
            }
        }


        public void resetEnvironment() {
            MapController.Instance.createVirtualMatrix();
            // handleSpawnBreakableObject();

            for (int i = 0; i < MapObjectManager.Instance.breakableObjectContainer.childCount; ++i)
            {
                GameObject go = MapObjectManager.Instance.breakableObjectContainer.GetChild(i).gameObject;
                go.GetComponent<Environment.BreakableObject>().setCurrentPoint(Environment.GameManager.Instance.getEmptyPoint());
            }

            for (int i = 0; i < MapObjectManager.Instance.characterContainer.childCount; ++i)
            {
                MapObjectManager.Instance.characterContainer.GetChild(i).GetComponent<CharacterController>().setCurrentPoint(Environment.GameManager.Instance.getEmptyPoint(), false);
            }
        }

        private void handleSpawnCharacters()
        {
            agent1 = MapObjectManager.Instance.spawnCharacter(1115, 1);
            MapObjectManager.Instance.spawnCharacter(1115, 2);
        }

        private void handleSpawnBreakableObject() {
            MapObjectManager.Instance.spawnBreakableObjects(0, 0);

            agent1.GetComponent<DashingGameAgent>().enabled = true;
        }

        public void resetTimer()
        {
            timeLeft = timerStart;
        }
    }
}