using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace FYP.InGame.Map
{
    public class SpawnPoint {
        [XmlElement("x")]
        public int x;
        [XmlElement("y")]
        public int y;

        public Point getPoint() { 
            return new Point(x, y);
        }
    }

    public class BreakableObject {
        [XmlAttribute("name")]
        public string name;

        [XmlArray("spawnPoints")]
        [XmlArrayItem("point")]
        public List<SpawnPoint> objectSpawnPoints = new List<SpawnPoint>();
    }

    [XmlRoot("mapInitPoints")]
    public class MapInitPoints {
        [XmlArray("breakableObjectSpawnPoints")]
        [XmlArrayItem("breakableObject")]
        public List<BreakableObject> breakableObjectSpawnPoints = new List<BreakableObject>();

        [XmlArray("playerSpawnPoints")]
        [XmlArrayItem("spawnPoint")]
        public List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();


    }
}