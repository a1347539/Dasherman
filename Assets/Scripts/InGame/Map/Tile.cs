using FYP.InGame.PlayerInstance;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.Map
{
    public class Tile
    {
        public enum TileStates {
            empty = 0,
            hasBreakable = 1,
            hasUnbreakable = 2,
            hasPlayer = 3
        };

        public Point point { get; private set; }

        public Vector2 worldPositionOfCellCenter { get; private set; }

        public TileStates tileState { get; private set; }

        public List<PhotonView> currentObjects { get; private set; }

        public Tile(Point p, Vector2 worldPos) {
            currentObjects = new List<PhotonView>();
            point = p;
            float offset = MapController.Instance.cellSize / 2;
            worldPositionOfCellCenter = new Vector2(
                worldPos.x + offset,
                worldPos.y - offset
                );
            tileState = TileStates.empty;
        }

        public void objectEnter(PhotonView obj) {
            currentObjects.Add(obj);
            if (obj.GetComponent<CharacterVital>() != null)
            {
                tileState = TileStates.hasPlayer;
            }
            else if (obj.GetComponent<InGame.BreakableObject.BreakableObject>() != null) {
                tileState = TileStates.hasBreakable;
            }
        }

        public void objectExit(PhotonView obj)
        {
            currentObjects.Remove(obj);
            if (currentObjects.Count == 0) {
                tileState = TileStates.empty;
            }
        }
    }
}