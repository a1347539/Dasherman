using FYP.Global;
using FYP.Global.InGame.Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FYP.Upgrade.Graph;

namespace FYP.Upgrade
{
        public class GraphManager : Singleton<GraphManager>
    {
        [SerializeField]
        public Transform graphCanvas;
        [SerializeField]
        public Transform nodeContainer;
        [SerializeField]
        public Transform lineContainer;
        [SerializeField]
        public GameObject upgradeNodePrefab;

        public TextAsset[] graphDatas;

        public Dictionary<int, Node> intNodePair = new Dictionary<int, Node>();

        Graph graph;

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
        };

        void Start()
        {
            graphDatas = Resources.LoadAll<TextAsset>(GraphKeys.resourceObjectPathPrefix);
            AdjacencyList adjacencyList = XmlUtilities.load<AdjacencyList>(graphDatas[0]);
            print(adjacencyList.nodes[1].Id);
            //graph = new Graph(intNodePair);
            //graph.createGraph(adjacencyList);
            // testing
        }


        void Update()
        {

        }

        public Node intToNode(int i) {
            return intNodePair[i];
        }
    }
}