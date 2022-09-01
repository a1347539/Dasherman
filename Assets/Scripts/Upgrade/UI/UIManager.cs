using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FYP.Upgrade
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject upgradeInfoContentPrefab;

        private GameObject currentUpgradeInfo;

        private void Awake()
        {
            GraphManager.onUpgradeNodeClick += handleDisplayUpgradeInfoContent;
        }

        private void OnDestroy()
        {
            GraphManager.onUpgradeNodeClick -= handleDisplayUpgradeInfoContent;
        }

        private void handleDisplayUpgradeInfoContent(int nodeID) {

            currentUpgradeInfo = Instantiate(upgradeInfoContentPrefab);
            UpgradeNode n = GraphManager.Instance.intNodePair[nodeID].prefab.GetComponent<UpgradeNode>();
            currentUpgradeInfo.GetComponent<UpgradeInfoContent>().initialize(n.nodeData, n.isUpgradeable);
            
        }

        public void handleCloseUpgradeInfoContent() {
            currentUpgradeInfo.GetComponent<UpgradeInfoContent>().close();
        }
    }
}