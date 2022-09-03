using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FYP.Upgrade
{
    public class ChildNodeID {
        [XmlElement("id")]
        public int id;
        [XmlElement("requisite")]
        public int levelRequisite;
    }

    public class NodeData
    {
        [XmlElement("id")]
        public int id;
        [XmlElement("upgradeType")]
        public int upgradeType;
        [XmlElement("value")]
        public float value;
        [XmlElement("cost")]
        public int cost;
        [XmlElement("levelRequirement")]
        public int levelRequirement;
        [XmlElement("maxLevel")]
        public int maxLevel;
        [XmlElement("name")]
        public string name;
        [XmlElement("description")]
        public string description;
        [XmlArray("children")]
        [XmlArrayItem("child")]
        public List<ChildNodeID> children = new List<ChildNodeID>();
    }

    [XmlRoot("adjacencyList")]
    public class AdjacencyList
    {
        [XmlArray("list")]
        [XmlArrayItem("node")]
        public List<NodeData> nodes = new List<NodeData>();
    }
}