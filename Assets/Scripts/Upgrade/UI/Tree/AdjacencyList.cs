using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FYP.Upgrade
{
    public class ChildNodeID {
        [XmlElement("id")]
        public int Id;
    }

    public class NodeData
    {
        [XmlElement("id")]
        public int Id;
        [XmlElement("upgrade")]
        public int upgradeType;
        [XmlElement("value")]
        public float Value;
        [XmlElement("cost")]
        public int cost;
        [XmlElement("levelRequirement")]
        public int levelRequirement;
        [XmlElement("name")]
        public string Name;
        [XmlElement("description")]
        public string description;
        [XmlArray("children")]
        [XmlArrayItem("id")]
        public List<ChildNodeID> children;
    }

    [XmlRoot("adjacencyList")]
    public class AdjacencyList
    {
        [XmlArray("list")]
        [XmlArrayItem("node")]
        public List<NodeData> nodes = new List<NodeData>();
    }
}