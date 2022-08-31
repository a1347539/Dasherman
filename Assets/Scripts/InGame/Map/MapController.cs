using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FYP.Global;
using FYP.Global.InGame;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;
using System.Text;
using FYP.InGame.PlayerInstance;
using Photon.Realtime;
using FYP.Global.Photon;

namespace FYP.InGame.Map
{
    public class MapController : Singleton<MapController>
    {
        public static Action onMapLoaded = delegate { };

        [SerializeField]
        private ScriptableMap[] mapDatas;

        public ScriptableMap mapData { get; private set; }
        public MapInitPoints initPoints { get; private set; }

        private List<Grid> grids;
        public Grid playableGrid { get; private set; }
        public Tilemap playableTilemapBaseLayer { get; private set; }
        public Vector2 playableAreaOrigin { get; private set; }
        public Vector2Int playableMapSize { get; private set; }
        public float cellSize { get; private set; }

        public List<List<Tile>> tileMatrix = new List<List<Tile>>();

        public Vector2 characterSpriteScaling { get { return new Vector2(cellSize * 2.6f, cellSize * 2.6f); } }

        public float characterSpriteOffsetInY { get { return cellSize * 0.3f; } }

        public Vector2 objectSpriteScaling { get { return new Vector2(cellSize * 1.4f, cellSize * 1.4f); } }

        public float objectSpriteOffsetInY { get { return cellSize * -0.2f; } }

        private void Awake()
        {
            GameManager.onGameSetup += handleInitializeMap;
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        private void OnDestroy()
        {
            GameManager.onGameSetup -= handleInitializeMap;
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }

        private void handleInitializeMap()
        {
            mapDatas = Resources.LoadAll<ScriptableMap>(MapKeys.scriptableMapPathPrefix);
            mapData = mapDatas[0];
            GameObject obj = NetworkUtilities.networkInstantiate(mapData.mapPrefab, Vector3.zero, Quaternion.identity, true);
            grids = new List<Grid>(mapData.mapPrefab.GetComponentsInChildren<Grid>());
            initPoints = XmlUtilities.load<MapInitPoints>(mapData.mapInitPoints);
            playableGrid = grids.FirstOrDefault(map => map.CompareTag(MapKeys.mapReachableTage));

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

        public void createVirtualMatrix() {
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

        public void entryToTile(Point p, PhotonView pv) {
            PhotonEvents.InGameEvents.updateTileMatrixEvent(p, true, pv.ViewID);
        }

        public void exitFromTile(Point p, PhotonView pv) {
            PhotonEvents.InGameEvents.updateTileMatrixEvent(p, false, pv.ViewID);
        }

        public Tile pointToTile(Point p)
        {
            return tileMatrix[p.y][p.x];
        }

        public void onTileDebugButtonClick() {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            List<Vector2> t = new List<Vector2>();
            for (int i = 0; i < tileMatrix.Count; i++)
            {
                for (int j = 0; j < tileMatrix[0].Count; j++)
                {
                    if (tileMatrix[i][j].tileState == Tile.TileStates.empty)
                    {
                        sb.Append('0');
                    }
                    else if (tileMatrix[i][j].tileState == Tile.TileStates.hasPlayer)
                    {
                        if (tileMatrix[i][j].currentObjects.Count > 1) {
                            t.Add(new Vector2(j, i));
                            count++; 
                        }
                        sb.Append('1');
                    }
                    else {
                        sb.Append('2');
                    }

                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
            Debug.Log($"there exist {count} tiles with at least 2 players in it");
            foreach (Vector2 v in t)
            {
                print(v);
            }
        }

        private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
        {
            if (obj.Code == PhotonCodes.updateTileMatrixEvent)
            {
                object[] content = (object[])obj.CustomData;
                Point p = (Point)content[0];
                bool isEntry = (bool)content[1];
                PhotonView pv = PhotonView.Find((int)content[2]);
                if (isEntry)
                {
                    print($"adding {pv.ViewID} to {p.x}, {p.y}");
                    tileMatrix[p.y][p.x].objectEnter(pv);
                }
                else
                {
                    print($"removing {pv.ViewID} from {p.x}, {p.y}");
                    tileMatrix[p.y][p.x].objectExit(pv);
                }
            }
        }

    }
}