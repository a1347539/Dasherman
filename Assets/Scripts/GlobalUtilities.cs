using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Xml.Serialization;
using System.IO;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using PlayFab;
using Newtonsoft.Json;

namespace FYP.Global
{
    public class NetworkUtilities
    {
        private static List<NetworkedPrefab> networkedPrefabs = new List<NetworkedPrefab>();

        public static GameObject networkInstantiate(GameObject obj, Vector3 position, Quaternion rotation, bool isRoomObj)
        {
            foreach (NetworkedPrefab networkedPrefab in networkedPrefabs)
            {
                if (networkedPrefab.prefab == obj)
                {
                    if (!isRoomObj)
                    {
                        return PhotonNetwork.Instantiate(networkedPrefab.path, position, rotation);
                    }
                    else 
                    {
                        return PhotonNetwork.InstantiateRoomObject(networkedPrefab.path, position, rotation);
                    }
                }
            }
            return null;
        }

        public static void loadNetworkPrefabs(string prefix) {
            GameObject[] resources = Resources.LoadAll<GameObject>(prefix + Path.DirectorySeparatorChar);
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].GetComponent<PhotonView>() != null) {
                    string objectName = resources[i].name;
                    NetworkedPrefab p = new NetworkedPrefab(resources[i], prefix + Path.DirectorySeparatorChar + objectName);
                    networkedPrefabs.Add(p);
                }
            }
        }

        public static object getCustomProperty(Player player, string key)
        {
            return player.CustomProperties[key];
        }

        public static void setCustomProperty(Player player, string key, object value)
        {
            PhotonHashtable table = new PhotonHashtable();
            table[key] = value;
            player.SetCustomProperties(table);
        }

        /*        public static bool isSameTeam(Player thisPlayer, Player otherPlayer) {
                    // return false;
                    if (thisPlayer.GetPhotonTeam().Code == otherPlayer.GetPhotonTeam().Code)
                    {
                        return true;
                    }
                    return false;
                }*/
    }

    public class InputUtilities 
    {
        public struct TouchData
        {
            public int touchID;
            public Vector2 onTouchDownPosition;
            public Vector2 onTouchDownWorldPosition;
            public TouchData(int id, Vector2 pos) { touchID = id; onTouchDownPosition = pos; onTouchDownWorldPosition = Camera.main.ScreenToWorldPoint(onTouchDownPosition);  }
            public float getAngleFromOnTouch(Vector2 currentPos)
            {
                Vector2 normalizedPosition = currentPos - onTouchDownPosition;
                if (normalizedPosition != Vector2.zero)
                {
                    float angleFromTouchToCurrent = (float)((Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) / Mathf.PI) * 180f);
                    if (angleFromTouchToCurrent < 0) angleFromTouchToCurrent += 360f;
                    return angleFromTouchToCurrent;
                }
                return -1;
            }
            public float getDistanceFromOnTouch(Vector2 currentPos)
            {
                return Vector2.Distance(onTouchDownPosition, currentPos);
            }
            public Vector2 getDeltaFromCurrentToOrigin(Vector2 current)
            {
                return new Vector2(current.x - onTouchDownPosition.x, current.y - onTouchDownPosition.y);
            }
            public Vector2 getDeltaFromCurrentToOriginInWorldSpace(Vector2 current)
            {
                return current - onTouchDownWorldPosition;
            }
        }

        public struct MouseButtonData
        {
            public Vector2 onButtonDownPosition;
            public Vector2 onButtonDownWorldPosition;
            public MouseButtonData(Vector2 pos) { onButtonDownPosition = pos; onButtonDownWorldPosition = Camera.main.ScreenToWorldPoint(onButtonDownPosition); }
            public float getAngleFromOnTouch(Vector2 currentPos)
            {
                Vector2 normalizedPosition = currentPos - onButtonDownPosition;
                if (normalizedPosition != Vector2.zero)
                {
                    float angleFromTouchToCurrent = (float)((Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) / Mathf.PI) * 180f);
                    if (angleFromTouchToCurrent < 0) angleFromTouchToCurrent += 360f;
                    return angleFromTouchToCurrent;
                }
                return -1;
            }
            public float getDistanceFromOnTouch(Vector2 currentPos)
            {
                return Vector2.Distance(onButtonDownPosition, currentPos);
            }
            public Vector2 getDeltaFromCurrentToOrigin(Vector2 current)
            {
                return current - onButtonDownPosition;
            }
            public Vector2 getDeltaFromCurrentToOriginInWorldSpace(Vector2 current)
            {
                return current - onButtonDownWorldPosition;
            }
        }
    }

    public class XmlUtilities
    {
        public static T load<T>(TextAsset textAsset)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(textAsset.text);
            T obj = (T)serializer.Deserialize(stringReader);
            stringReader.Close();
            return obj;
        }
    }

    public class JsonUtilities
    {
        public static string serialize<T>(T obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public static T deserialize<T>(string obj) {
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }

    public class EnumUtilities {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}