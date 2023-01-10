using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using System.Text;
using FYP.Global.InGame;

namespace FYP.InGame.AI.Environment
{
    public class MapController : Singleton<MapController>
    {
        public static Action onMapLoaded = delegate { };

        [SerializeField]
        private Map.ScriptableMap[] mapDatas;
        public Map.ScriptableMap mapData { get; private set; }
        private List<Grid> grids;
        public Grid playableGrid { get; private set; }
        public Vector2Int playableMapSize { get; private set; }
        public float cellSize { get; private set; }
        public Tilemap playableTilemapBaseLayer { get; private set; }
        public Vector2 playableAreaOrigin { get; private set; }

        public List<List<Tile>> tileMatrix = new List<List<Tile>>();

        public Vector2 characterSpriteScaling { get { return new Vector2(cellSize * 2.6f, cellSize * 2.6f); } }
        public float characterSpriteOffsetInY { get { return cellSize * 0.3f; } }
        public Vector2 objectSpriteScaling { get { return new Vector2(cellSize * 1.4f, cellSize * 1.4f); } }
        public float objectSpriteOffsetInY { get { return cellSize * -0.2f; } }

        void Start()
        {
            initializeMap();
        }

        private void initializeMap() {
            mapDatas = Resources.LoadAll<Map.ScriptableMap>(AIKeys.AIEnvironmentPathPrefix);
            mapData = mapDatas[0];
            GameObject obj = Instantiate(mapData.mapPrefab, Vector3.zero, Quaternion.identity);

            grids = new List<Grid>(mapData.mapPrefab.GetComponentsInChildren<Grid>());
            playableGrid = grids.FirstOrDefault(map => map.CompareTag(MapKeys.mapReachableTag));

            playableMapSize = mapData.playableMapSize;
            // cell is square
            cellSize = playableGrid.cellSize.x;
            playableTilemapBaseLayer = playableGrid.transform.GetChild(0).GetComponent<Tilemap>();

            playableAreaOrigin = new Vector2(
                playableTilemapBaseLayer.cellBounds.xMin * cellSize,
                playableTilemapBaseLayer.cellBounds.yMax * cellSize
                );
            createVirtualMatrix();
            onMapLoaded?.Invoke();
        }

        public void createVirtualMatrix()
        {
            for (int i = 0; i < mapData.playableMapSize.y; i++)
            {
                tileMatrix.Add(new List<Tile>());
                for (int j = 0; j < mapData.playableMapSize.x; j++)
                {
                    Tile tile = new Tile(new Point(j, i), new Vector2(
                        playableAreaOrigin.x + cellSize * j, playableAreaOrigin.y - cellSize * i
                        ));

                    tileMatrix[i].Add(tile);
                }
            }
        }

        public void printMatrix() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tileMatrix.Count; i++)
            {
                for (int j = 0; j < tileMatrix[0].Count; j++)
                {
                    if (tileMatrix[i][j].tileState == Tile.TileStates.empty)
                    {
                        sb.Append('0');
                    }
                    else
                    {
                        sb.Append('1');
                    }
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        public void entryToTile(Point p, GameObject gameObject)
        {
            tileMatrix[p.y][p.x].objectEnter(gameObject);
        }

        public void exitFromTile(Point p, GameObject gameObject)
        {
            tileMatrix[p.y][p.x].objectExit(gameObject);
        }

        public Tile pointToTile(Point p)
        {
            return tileMatrix[p.y][p.x];
        }
    }
}