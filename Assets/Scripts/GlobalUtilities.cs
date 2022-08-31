using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Xml.Serialization;
using System.IO;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

namespace FYP.Global
{
    public class NetworkUtilities : MonoBehaviour
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

    public class XmlUtilities : MonoBehaviour
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
}