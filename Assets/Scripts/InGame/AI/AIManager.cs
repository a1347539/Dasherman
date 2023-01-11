using FYP.InGame.AI.Environment;
using System;
using UnityEngine;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;
using FYP.InGame.AI.Agent;
using System.Collections.Generic;
using FYP.InGame.AI.Environment.Character;
using System.Collections;
using Unity.MLAgents;

namespace FYP.InGame.AI
{
    public class AIManager : MonoBehaviour
    {
        public Action onTimerEnd = delegate { };

        [SerializeField]
        private MapObjectManager mapObjectManager;
        [SerializeField]
        private MapController mapController;
        [SerializeField]
        private Environment.GameManager gameManager;

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
            mapObjectManager.spawnCharacterAvailable += handleSpawnCharacters;
            mapObjectManager.spawnBreakableObjectAvailable += handleSpawnBreakableObject;
            mapController.onMapLoaded += handleMapLoaded;
        }

        private void Start()
        {
            timeLeft = timerStart;
            durationInStep = timerStart / Time.fixedDeltaTime;
        }

/*        void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                onTimerEnd?.Invoke();
            }
        }*/

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
            List<List<Tile>> tiles = mapController.tileMatrix;
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
            mapController.createVirtualMatrix();

            for (int i = 0; i < mapObjectManager.breakableObjectContainer.childCount; ++i)
            {
                GameObject go = mapObjectManager.breakableObjectContainer.GetChild(i).gameObject;
                go.GetComponent<Environment.BreakableObject>().setCurrentPoint(gameManager.getEmptyPoint());
            }

            for (int i = 0; i < mapObjectManager.characterContainer.childCount; ++i)
            {
                mapObjectManager.characterContainer.GetChild(i).GetComponent<CharacterController>().setCurrentPoint(gameManager.getEmptyPoint(), false);
                mapObjectManager.characterContainer.GetChild(i).GetComponent<CharacterVital>().setMana(
                    mapObjectManager.characterContainer.GetChild(i).GetComponent<CharacterVital>().maxMana
                    );
            }

            getNewMap();
            isResetingEnvironment = false;
        }

        private void handleMapLoaded() {
            StartCoroutine(continuousGetNewMapCoroutine());
            StartCoroutine(lateEnableAgentsCoroutine());
        }

        private void handleSpawnCharacters()
        {
            agent1 = mapObjectManager.spawnCharacter(1115, 1);
            mapObjectManager.spawnCharacter(1115, 2);
        }

        private void handleSpawnBreakableObject() {
            mapObjectManager.spawnBreakableObjects(0, 0);
        }

        public void resetTimer()
        {
            timeLeft = timerStart;
        }
    }
}