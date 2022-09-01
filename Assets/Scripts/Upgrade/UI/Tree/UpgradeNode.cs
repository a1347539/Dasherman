using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FYP.Upgrade
{
    public class UpgradeNode : MonoBehaviour
    {
        // increment stay the same accross all levels, cost is multiplied by level and probably another constant factor

        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Sprite[] sprites;
        [SerializeField]
        private Button nodeButton;

        public bool isUpgradeable;

        #region static
        public NodeData nodeData { get; private set; }
        public int nodeId { get; private set; }
        public int upgradeType { get; private set; }
        public int value { get; private set; }
        public int cost { get; private set; }
        public int levelRequirement { get; private set; }
        public string nodeName { get; private set; }
        public string description { get; private set; }
        #endregion

        #region player data
        public int level { get; private set; }

        #endregion


        private void Start()
        {
            nodeButton.onClick.AddListener(onUpgradeNodeClick);
        }

        public void initialize(NodeData data) {
            nodeData = data;
            nodeId = data.Id;
            text.text = data.Id.ToString();
            background.sprite = sprites[data.Id/100];
        }

        public void onSetInteractable(bool isUpgradeable) {
            this.isUpgradeable = isUpgradeable;
        }

        private void onUpgradeNodeClick() {
            GraphManager.onUpgradeNodeClick?.Invoke(nodeId);
        }
    }
}