using FYP.Global;
using FYP.Global.Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FYP.Upgrade.Graph;

namespace FYP.Upgrade
{
    public class GraphManager : Singleton<GraphManager>
    {
        public static Action onGraphCreated = delegate { };
        public static Action<int> onUpgradeNodeClick = delegate { };
        public static Action<int> onNodeUpgrade = delegate { };

        [SerializeField]
        public Transform graphCanvas;
        [SerializeField]
        public Transform nodeContainer;
        [SerializeField]
        public Transform lineContainer;
        [SerializeField]
        public GameObject upgradeNodePrefab;
        [SerializeField]
        public List<Sprite> nodebackgrounds;

        public TextAsset[] graphDatas;

        public Dictionary<int, Node> intNodePair = new Dictionary<int, Node>();

        public AdjacencyList adjacencyList { get; private set; }

        public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

        public List<UpgradeLevel> UpgradeLevels {
            set {
                upgradeLevels = value;
                onLevelSet(value);
            }
        }

        public Vector2 upgradeNodeSize { get {
                Rect rect = upgradeNodePrefab.GetComponent<RectTransform>().rect;
                return new Vector2(rect.width, rect.height);
            } }

        Graph graph;
        /*
                List<List<int>> adjacencyList = new List<List<int>> { 
                    new List<int> { 0, 101, 201, 301 },
                    new List<int> { 101, 102, 0, 103 },
                    new List<int> { 102, 104, 0, 105 },
                    new List<int> { 103, 0, 106, 0, 105 },
                    new List<int> { 104, 0, 107, 0},
                    new List<int> { 105, 0, 108, 0},
                    new List<int> { 106, 0, 0 , 109, 108},
                    new List<int> { 107, 113, 110, 0},
                    new List<int> { 108, 0, 111, 0},
                    new List<int> { 109 },
                    new List<int> { 110, 0, 0, 112},
                    new List<int> { 111, 0, 0, 0, 112},
                    new List<int> { 112 },
                    new List<int> { 113 },
                    new List<int> { 201 },
                    new List<int> { 301 },
                };*/

        private void Awake()
        {
            graphDatas = Resources.LoadAll<TextAsset>(GraphKeys.resourceObjectPathPrefix);
            adjacencyList = XmlUtilities.load<AdjacencyList>(graphDatas[0]);
        }

        private void onLevelSet(List<UpgradeLevel> upgradeLevels) {
            graph = new Graph();
            graph.createGraph(adjacencyList, upgradeLevels);
        }

        void Update()
        {

        }

        public Node intToNode(int i) {
            return intNodePair[i];
        }

        public void onDebugGraphButtonClick() {
            print("level: ");
            foreach (UpgradeLevel ul in upgradeLevels) {
                print(ul.id + " " + ul.level);
            }
            print("node:");
            foreach (KeyValuePair<int, Node> pair in intNodePair)
            {
                print($"{pair.Key} {pair.Value.level} levelreq. {pair.Value.levelRequirementIsMet} parentreq. {String.Join(", ", pair.Value.parentRequirementsAreMet)}");
            }
        }
    }
}