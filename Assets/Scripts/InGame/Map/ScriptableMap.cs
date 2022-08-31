using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.Map
{
    [CreateAssetMenu(fileName = "NewMap", menuName = "InGame/Map")]
    public class ScriptableMap : ScriptableObject
    {
        [SerializeField]
        public int mapIndex = -1;
        [SerializeField]
        public string mapName;
        [SerializeField]
        public GameObject mapPrefab;
        [SerializeField]
        public Vector2Int playableMapSize;

        [SerializeField]
        public List<BreakableTile> breakableTilePrefabs;
        [SerializeField]
        public TextAsset mapInitPoints;

    }
}