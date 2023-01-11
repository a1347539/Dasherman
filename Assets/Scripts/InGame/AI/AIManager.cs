using FYP.InGame.AI.Environment;
using System;
using UnityEngine;
using FYP.InGame.AI;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;
using FYP.InGame.AI.Agent;
using System.Collections.Generic;
using FYP.InGame.AI.Environment.Character;
using System.Collections;
using Unity.MLAgents;

namespace FYP.InGame.AI
{
    public class AIManager : Singleton<AIManager>
    {
        public Action onTimerEnd = delegate { };

        private bool isResetingEnvironment = false;

        public List<int> processedTileMatrix;

        public float timerStart;
        private float timeLeft;

        public float durationInStep;

        public float microReward = 1 / 500f;
        public float reward = 1f;

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
            StartCoroutine(continuousGetNewMapCoroutine());
            StartCoroutine(lateEnableAgentsCoroutine());
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                onTimerEnd?.Invoke();
            }
        }

        public IEnumerator continuousGetNewMapCoroutine() {
            for (; ; )
            {
                while (isResetingEnvironment) { yield return null; }
                getNewMap();
                yield return new WaitForSeconds(3f);
            }
        }

        public IEnumerator lateEnableAgentsCoroutine() {
            yield return new WaitForSeconds(1f);
            agent1.GetComponent<DashingGameAgent>().enabled = true;
        }

        public void getNewMap() {
            processedTileMatrix = new List<int>();
            List<List<Tile>> tiles = MapController.Instance.tileMatrix;
            for (int i = 0; i < tiles.Count; ++i)
            {
                for (int j = 0; j < tiles[0].Count; ++j)
                {
                    int state = (int)tiles[i][j].tileState;
                    if (state != 3) { processedTileMatrix.Add(state); }
                    else
                    {
                        if (tiles[i][j].currentObjects.Count == 1)
                        {
                            processedTileMatrix.Add(tiles[i][j].currentObjects[0].GetComponent<CharacterBuilder>().teamNumber);
                        }
                        else
                        {
                            // having two character on the same tile is an exception, will treat it as an unbreakable block
                            processedTileMatrix.Add((int)Tile.TileStates.hasUnbreakable);
                        }
                    }
                }
            }
        }


        public void resetEnvironment() {
            isResetingEnvironment = true;
            MapController.Instance.createVirtualMatrix();

            for (int i = 0; i < MapObjectManager.Instance.breakableObjectContainer.childCount; ++i)
            {
                GameObject go = MapObjectManager.Instance.breakableObjectContainer.GetChild(i).gameObject;
                go.GetComponent<Environment.BreakableObject>().setCurrentPoint(Environment.GameManager.Instance.getEmptyPoint());
            }

            for (int i = 0; i < MapObjectManager.Instance.characterContainer.childCount; ++i)
            {
                MapObjectManager.Instance.characterContainer.GetChild(i).GetComponent<CharacterController>().setCurrentPoint(Environment.GameManager.Instance.getEmptyPoint(), false);
            }

            getNewMap();
            isResetingEnvironment = false;
        }

        private void handleSpawnCharacters()
        {
            agent1 = MapObjectManager.Instance.spawnCharacter(1115, 1);
            MapObjectManager.Instance.spawnCharacter(1115, 2);
        }

        private void handleSpawnBreakableObject() {
            MapObjectManager.Instance.spawnBreakableObjects(0, 0);
        }

        public void resetTimer()
        {
            timeLeft = timerStart;
        }
    }
}