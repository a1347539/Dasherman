using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FYP.Upgrade.Graph;

namespace FYP.Upgrade
{
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        public static Action<Node, bool, bool> onNodeUpgraded = delegate { };

        public int playerLevel;
        private int playerGold;
        public int PlayerGold { get { return playerGold; }
            set {
                playerGold = value;
                UIManager.Instance.setGoldText(value);
            } }

        private void Awake()
        {
            GraphManager.onNodeUpgrade += handleNodeUpgrade;
        }

        private void OnDestroy()
        {
            GraphManager.onNodeUpgrade -= handleNodeUpgrade;
        }

        private void handleNodeUpgrade(int id) {
            print($"upgrading {id}");
            Node node = GraphManager.Instance.intNodePair[id];
            if (!node.parentRequirementsAreMet.TrueForAll(x => x) || !node.levelRequirementIsMet || node.cost > PlayerGold)
            {
                print("not upgradeable");
                return;
            }

            // decrease player gold
            playerGold -= node.cost;
            UIManager.Instance.setGoldText(playerGold);
            // change current node
            // change the children and parent of current node
            // change intNodePair
            node.reInitialize(node.level + 1);

            // change upgradeLevels
            ++GraphManager.Instance.upgradeLevels.First(ul => ul.id == id).level;

            // save data to playfab
            PlayFabUpgradeManager.Instance.saveData();
            onNodeUpgraded?.Invoke(node, PlayerGold >= node.cost, node.level >= node.nodeData.maxLevel);
        }
    }
}