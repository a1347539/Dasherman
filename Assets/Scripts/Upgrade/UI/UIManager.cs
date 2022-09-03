using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FYP.Upgrade.Graph;

namespace FYP.Upgrade
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private TMP_Text goldText;

        [SerializeField]
        private GameObject upgradeInfoContentPrefab;
        [SerializeField]
        private Transform upgradeInfoContentViewContainer;
        [SerializeField]
        private Button GraphOverlay;

        private GameObject currentUpgradeInfo;

        private void Awake()
        {
            GraphManager.onUpgradeNodeClick += handleDisplayUpgradeInfoContent;
            GraphOverlay.onClick.AddListener(handleCloseUpgradeInfoContent);
        }

        private void OnDestroy()
        {
            GraphManager.onUpgradeNodeClick -= handleDisplayUpgradeInfoContent;
            GraphOverlay.onClick.RemoveAllListeners();
        }

        public void setGoldText(int value) {
            goldText.text = value.ToString("C0", CultureInfo.CreateSpecificCulture("en-US"));
        }

        private void handleDisplayUpgradeInfoContent(int nodeID) {

            Node node = GraphManager.Instance.intNodePair[nodeID];
            currentUpgradeInfo = Instantiate(upgradeInfoContentPrefab, node.prefab.transform.position, Quaternion.identity, upgradeInfoContentViewContainer);
            currentUpgradeInfo.GetComponent<UpgradeInfoContent>().initialize(node);
            GraphOverlay.gameObject.SetActive(true);
        }

        public void handleCloseUpgradeInfoContent() {
            if (currentUpgradeInfo != null)
            {
                currentUpgradeInfo.GetComponent<UpgradeInfoContent>().close();
                currentUpgradeInfo = null;
            }
            GraphOverlay.gameObject.SetActive(false);
        }
    }
}