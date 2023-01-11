using FYP.InGame.AI.Environment.Character;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public class Tile
    {
        public enum TileStates
        {
            empty = 0,
            hasBreakable = 1,
            hasUnbreakable = 2,
            hasPlayer = 3
        };

        public Point point { get; private set; }

        public Vector2 worldPositionOfCellCenter { get; private set; }

        public TileStates tileState { get; private set; }

        public List<GameObject> currentObjects { get; private set; }

        public Tile(Point p, Vector2 worldPos, MapController mapController)
        {
            currentObjects = new List<GameObject>();
            point = p;
            float offset = mapController.cellSize / 2;
            worldPositionOfCellCenter = new Vector2(
                worldPos.x + offset,
                worldPos.y - offset
                );
            tileState = TileStates.empty;
        }

        public void objectEnter(GameObject obj)
        {
            currentObjects.Add(obj);
            if (obj.GetComponent<CharacterVital>() != null)
            {
                tileState = TileStates.hasPlayer;
            }
            else if (obj.GetComponent<BreakableObject>() != null)
            {
                tileState = TileStates.hasBreakable;
            }
        }

        public void objectExit(GameObject obj)
        {
            currentObjects.Remove(obj);
            if (currentObjects.Count == 0)
            {
                tileState = TileStates.empty;
            }
        }
    }
}