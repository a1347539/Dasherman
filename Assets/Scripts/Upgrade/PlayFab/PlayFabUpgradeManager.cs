using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System;
using FYP.Global;

namespace FYP.Upgrade
{
    public class PlayFabUpgradeManager : Singleton<PlayFabUpgradeManager>
    {
        private void Awake()
        {
            DebugConnection.onConnectedToPlayFab += handleOnConnected;
        }

        private void OnDestroy()
        {
            DebugConnection.onConnectedToPlayFab -= handleOnConnected;
        }

        private void handleOnConnected() {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                Keys = new List<string> {
                    PlayFabKeys.PlayerGold,
                    PlayFabKeys.PlayerLevel,
                    PlayFabKeys.PlayerUpgrades,
                }
            },
                onGetData, 
                onError
            );
        }

        private void onGetData(GetUserDataResult result)
        {
            if (result.Data.ContainsKey(PlayFabKeys.PlayerLevel) && result.Data.ContainsKey(PlayFabKeys.PlayerGold))
            {
                UpgradeManager.Instance.playerLevel = int.Parse(result.Data[PlayFabKeys.PlayerLevel].Value);
                UpgradeManager.Instance.PlayerGold = int.Parse(result.Data[PlayFabKeys.PlayerGold].Value);
            }

            if (result.Data.ContainsKey(PlayFabKeys.PlayerUpgrades))
            {
                List<UpgradeLevel> levels = JsonUtilities.deserialize<List<UpgradeLevel>>(result.Data[PlayFabKeys.PlayerUpgrades].Value);

                if (levels.Count != GraphManager.Instance.adjacencyList.nodes.Count)
                {
                    for (int i = 0; i < GraphManager.Instance.adjacencyList.nodes.Count; ++i)
                    {
                        if (i >= levels.Count || GraphManager.Instance.adjacencyList.nodes[i].id != levels[i].id) 
                        {
                            levels.Insert(i, new UpgradeLevel(GraphManager.Instance.adjacencyList.nodes[i].id, 0));
                        }
                    }
                }
                GraphManager.Instance.UpgradeLevels = levels;
            }
            else
            {
                initializeUpgradeLevelsInPlayFab();
            }
        }

        private void initializeUpgradeLevelsInPlayFab() {
            List<UpgradeLevel> levels = new List<UpgradeLevel>();
            // initialize data in playfab
            foreach (NodeData data in GraphManager.Instance.adjacencyList.nodes) levels.Add(new UpgradeLevel(data.id, 0));
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string> {
                    { PlayFabKeys.PlayerUpgrades, JsonUtilities.serialize(levels) }
                }
            }, result => { 
                print("successful save data"); 
                GraphManager.Instance.UpgradeLevels = levels;
            },
            onError
            );
        }

        public void saveData() {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string> {
                    { PlayFabKeys.PlayerUpgrades, JsonUtilities.serialize(GraphManager.Instance.upgradeLevels) },
                    { PlayFabKeys.PlayerGold, UpgradeManager.Instance.PlayerGold.ToString() },
                }
            }, result => { print("successful save data"); },
                onError
                );
        }

        public void onError(PlayFabError obj)
        {
            Debug.Log(obj.GenerateErrorReport());
        }
    }
}