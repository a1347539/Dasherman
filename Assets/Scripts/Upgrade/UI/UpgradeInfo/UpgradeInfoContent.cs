using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.Upgrade
{
    public class UpgradeInfoContent : MonoBehaviour
    {
        [SerializeField]
        private Button upgradeButton;

        public void initialize(NodeData data, bool isUpgradeable) {
            upgradeButton.interactable = isUpgradeable;

        }

        public void close() {
            Destroy(gameObject);
        }
    }
}