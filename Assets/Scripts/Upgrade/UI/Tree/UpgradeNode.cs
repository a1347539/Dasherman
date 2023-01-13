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
        [Description("Health Point")]
        HealthPoint = 0,
        [Description("Mana Point")]
        ManaPoint = 1,
        [Description("Physical Damage")]
        PhysicalDamage = 2,
        [Description("Magic Damage")]
        MagicDamage = 3,
        [Description("Physical Defence")]
        PhysicalDefence = 4,
        [Description("Magic Defence")]
        MagicDefence = 5,
        [Description("Mana Regeneration")]
        ManaRegeneration = 6,
        [Description("Skill")]
        Skill = 101,
    }

    public enum UpgradeTypeTitles
    {
        Strength = 0,
        Dexterity = 1,
        Intelligence = 2,
        Skill = 101,
    }

    public class UpgradeNode : MonoBehaviour
    {
        // increment stay the same accross all levels, cost is multiplied by level and probably another constant factor

        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Button nodeButton;

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
            background.sprite = GraphManager.Instance.nodebackgrounds[data.id/100];

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
            if (id == 0) return;
            GraphManager.onUpgradeNodeClick?.Invoke(nodeData.id);
        }
    }
}