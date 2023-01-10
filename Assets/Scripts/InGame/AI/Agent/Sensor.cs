using FYP.InGame.AI.Environment;
using FYP.InGame.AI.Environment.Character;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP.InGame.AI.Agent
{
    public class Sensor : MonoBehaviour
    {
        // observe map
        public static bool isTileMatrixProcessed = false;
        public static bool isProcessing = false;
        public static List<int> processedTileMatrix;

        public static void processTileMatrix()
        {
            isTileMatrixProcessed = false;
            isProcessing = false;

            if (isProcessing) { return; }
            isProcessing = true;
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
            isTileMatrixProcessed = true;
        }


        public int getHealth() {
            return GetComponent<CharacterVital>().currentHealth;
        }

        public int getMana() {
            return GetComponent<CharacterVital>().currentMana;
        }

        public Vector2 getSelfPosition() {
            Point p = GetComponent<Environment.Character.CharacterController>().currentPoint;
            return new Vector2(p.x, p.y);
        }
    }
}