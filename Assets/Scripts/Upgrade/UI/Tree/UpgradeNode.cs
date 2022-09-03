using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;
using static FYP.Upgrade.Graph;
using System.Linq;

namespace FYP.Upgrade
{
    public enum UpgradeTypes {
        [Description("Skill")]
        Skill = 0,
        [Description("Health Point")]
        HealthPoint = 1,
        [Description("Mana Point")]
        ManaPoint = 2,
        [Description("Physical Damage")]
        PhysicalDamage = 3,
        [Description("Magic Damage")]
        MagicDamage = 4,
        [Description("Physical Defence")]
        PhysicalDefence = 5,
        [Description("Magic Defence")]
        MagicDefence = 6,
        [Description("Mana Regeneration")]
        ManaRegeneration = 7,
    }

    public enum UpgradeTypeTitles
    {
        Skill = 0,
        Strength = 1,
        Dexterity = 2,
        Intelligence = 3,
    }

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

        public bool isUpgradeable { get; private set; }

        #region static
        public int id;
        public NodeData nodeData { get; private set; }
        #endregion


        private void Start()
        {
            nodeButton.onClick.AddListener(onUpgradeNodeClick);
        }

        public void initialize(NodeData data, ref bool levelRequirementIsMet) {
            nodeData = data;
            text.text = data.id.ToString();
            background.sprite = sprites[data.id/100];

            levelRequirementIsMet = UpgradeManager.Instance.playerLevel >= data.levelRequirement;
        }

        public void onAddParent(Node parent, List<bool> r) {
            r.Add(parent.level >= parent.nodeData.children.First(child => child.id == id).levelRequisite);
        }

        public void updateRequirement(Node parent, int index, List<bool> r) {
            // call from parent
            print($"{id} updating: {parent.id} at {index + 1}, originally {r[index]}, new {parent.level} >= {parent.nodeData.children.First(child => child.id == id).levelRequisite}");
            r[index] = parent.level >= parent.nodeData.children.First(child => child.id == id).levelRequisite;
        }

        private void onUpgradeNodeClick() {
            GraphManager.onUpgradeNodeClick?.Invoke(nodeData.id);
        }
    }
}