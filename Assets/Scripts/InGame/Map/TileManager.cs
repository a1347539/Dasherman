// obsolete, never used in game

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.Map
{
    public class TileManager : Singleton<TileManager>
    {
        public static Action onTilesLoaded = delegate { };

        public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

        private void Awake()
        {
            MapController.onMapLoaded += handleCreateTiles;
        }

        private void OnDestroy()
        {
            MapController.onMapLoaded -= handleCreateTiles;
        }

        public Tile pointToTile(Point p) {
            Tile t;
            tiles.TryGetValue(p, out t);
            return t;
        }

        private void handleCreateTiles()
        {
            Vector2 origin = MapController.Instance.playableAreaOrigin;
            Vector2 mapSize = MapController.Instance.mapData.playableMapSize;
            float cellSize = MapController.Instance.cellSize;

            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    Point p = new Point(j, i);
                    Tile tile = new Tile(p, new Vector2(
                        origin.x + cellSize * j, origin.y - cellSize * i
                        ));

                    tiles.Add(p, tile);
                }
            }
            onTilesLoaded?.Invoke();
        }
    }
}