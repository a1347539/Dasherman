using FYP.InGame.AI.Environment.Character;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private MapController mapController;

        public bool isSameTeam(GameObject a, GameObject b) {
            return a.GetComponent<CharacterBuilder>().teamNumber == b.GetComponent<CharacterBuilder>().teamNumber;
        }


        public Point getEmptyPoint() {
            int x;
            int y;
            do
            {
                // print("get spawn point");
                x = Random.Range(0, mapController.playableMapSize.x);
                y = Random.Range(0, mapController.playableMapSize.y);
            }
            while (mapController.tileMatrix[y][x].tileState != Tile.TileStates.empty);
            return new Point(x, y);
        }
    }
}