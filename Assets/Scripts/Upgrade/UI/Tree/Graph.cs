using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.Upgrade
{
    public class Graph : MonoBehaviour
    {
        private Node root;
        private int radius;

        public class Node {
            private List<Node> parents = new List<Node>();
            private List<Node> children = new List<Node>();
            public int id { get; private set; }
            public GameObject prefab { get; private set; }
            public NodeData nodeData { get; private set; }

            public Node(int id, Vector2 position) {
                this.id = id;
                prefab = Instantiate(GraphManager.Instance.upgradeNodePrefab, position, Quaternion.identity, GraphManager.Instance.nodeContainer);
                print($"add {id}");
                GraphManager.Instance.intNodePair[id] = this;
            }
            public void lateInitialize(NodeData data) {
                nodeData = data;
                prefab.GetComponent<UpgradeNode>().initialize(data);
            }
            public void addParent(Node u) {
                parents.Add(u);
            }
            public void addChild(Node v) {
                

                children.Add(v);
            }
        }

        public Graph() {
            radius = 3;
        }

        public void createGraph(AdjacencyList adjacencyList) {
            createRoot(adjacencyList.nodes[0]);
            int closeness = 17;
            float radiansDivided = 2 * Mathf.PI / closeness;
            float radians;
            float vertical;
            float horizontal;
            for (int i = 1; i < adjacencyList.nodes.Count; ++i)
            {
                print($"get {adjacencyList.nodes[i].Id}");
                Node root = GraphManager.Instance.intNodePair[adjacencyList.nodes[i].Id];
                root.lateInitialize(adjacencyList.nodes[i]);

                for (int j = 0; j < adjacencyList.nodes[i].children.Count; ++j)
                {
                    if (GraphManager.Instance.intNodePair.ContainsKey(adjacencyList.nodes[i].children[j].Id))
                    {
                        addEdge(root, GraphManager.Instance.intNodePair[adjacencyList.nodes[i].children[j].Id]);
                        continue;
                    }
                    radians = radiansDivided * (2 * (j - 2) % closeness);
                    horizontal = Mathf.Sin(radians);
                    vertical = Mathf.Cos(radians);
                    Vector3 vectorFromOrigin = new Vector2(horizontal, vertical) * radius;
                    Vector3 pos = root.prefab.transform.position + vectorFromOrigin;
                    addEdge(root, new Node(adjacencyList.nodes[i].children[j].Id, pos));
                }
            }

        }

        private void createRoot(NodeData firstNode) {
            float radiansDivided = 2 * Mathf.PI / 3;
            float radians;
            float vertical;
            float horizontal;

            root = new Node(firstNode.Id, Vector2.zero);
            root.lateInitialize(firstNode);

            for (int i = 0; i < firstNode.children.Count; ++i)
            {
                // the first node branch away evenly, thats why it is divided by 3
                radians = radiansDivided * i;
                vertical = Mathf.Cos(radians);
                horizontal = Mathf.Sin(radians);
                Vector3 vectorFromOrigin = new Vector2(horizontal, vertical) * radius;
                Vector3 pos = root.prefab.transform.position + vectorFromOrigin;
                addEdge(root, new Node(firstNode.children[i].Id, pos));
            }
        }

        private void addEdge(Node u, Node v) {
            // from u to v
            Vector2 posA = u.prefab.transform.position;
            Vector2 posB = v.prefab.transform.position;
            
            GameObject obj = new GameObject("line", typeof(Image));
            obj.transform.SetParent(GraphManager.Instance.lineContainer, false);
            obj.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);

            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            Vector2 direction = (posB - posA).normalized;
            float distance = Vector2.Distance(posA, posB);

            rectTransform.sizeDelta = new Vector2(distance, 0.1f);
            rectTransform.anchoredPosition = posA + direction * distance * 0.5f;

            float angle = (float)(Math.Atan2(posB.y - posA.y, posB.x - posA.x) * (180 / Math.PI));

            rectTransform.localEulerAngles = new Vector3(0, 0, angle);

            u.addChild(v); v.addParent(u);
        }


    }
}