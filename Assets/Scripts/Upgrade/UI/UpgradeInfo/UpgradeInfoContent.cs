using FYP.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FYP.Upgrade.Graph;

namespace FYP.Upgrade
{
    public class UpgradeInfoContent : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text titleText;
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private GameObject costArea;
        [SerializeField]
        private TMP_Text costText;
        [SerializeField]
        private TMP_Text requirementNotMetMessage;

        [SerializeField]
        private Button upgradeButton;

        private Animator anim;

        private int id;

        private void Awake()
        {
            UpgradeManager.onNodeUpgraded += handleNodeUpgraded;
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            upgradeButton.onClick.AddListener(handleUpgradeNode);
        }

        private void OnDestroy()
        {
            upgradeButton.onClick.RemoveAllListeners();
            UpgradeManager.onNodeUpgraded -= handleNodeUpgraded;
        }

        public void initialize(Node node)
        {
            NodeData data = node.nodeData;
            id = data.id;

            Rect rect = GetComponent<RectTransform>().rect;
            transform.position = new Vector2(transform.position.x + (rect.width + GraphManager.Instance.upgradeNodeSize.x) / 2, transform.position.y);

            if (node.levelRequirementIsMet && node.parentRequirementsAreMet.TrueForAll(x => x)) {
                descriptionText.gameObject.SetActive(true);
                upgradeButton.interactable = true;
            }
            else {
                if (!node.levelRequirementIsMet)
                {
                    requirementNotMetMessage.text = "Level requirement not met.";
                }
                else {
                    requirementNotMetMessage.text = "Prerequisites are not met.";
                }
                requirementNotMetMessage.gameObject.SetActive(true);
            }
            if (node.cost > UpgradeManager.Instance.PlayerGold)
            {
                upgradeButton.interactable = false;
            }
            if (data.maxLevel == node.level) {
                upgradeButton.gameObject.SetActive(false);
                costArea.SetActive(false);
            }

            if (data.id != 0)
            { 
                switch ((UpgradeTypes)data.upgradeType)
                {
                    case UpgradeTypes.Skill:
                        titleText.text = UpgradeTypeTitles.Skill.ToString();
                        break;
                    case UpgradeTypes.HealthPoint:
                    case UpgradeTypes.PhysicalDamage:
                    case UpgradeTypes.PhysicalDefence:
                        titleText.text = UpgradeTypeTitles.Strength.ToString();
                        break;
                    case UpgradeTypes.MagicDamage:
                    case UpgradeTypes.MagicDefence:
                        titleText.text = UpgradeTypeTitles.Intelligence.ToString();
                        break;
                    case UpgradeTypes.ManaPoint:
                    case UpgradeTypes.ManaRegeneration:
                        titleText.text = UpgradeTypeTitles.Dexterity.ToString();
                        break;
                }
            }

            levelText.text = $"{node.level}/{data.maxLevel}";

            descriptionText.text = $"<style=\"Title\"><b>";
            if (data.upgradeType == 101)
            {
                descriptionText.text += $"{data.name}";
            }
            else
            {
                descriptionText.text += $"{EnumUtilities.GetEnumDescription((Enum)(UpgradeTypes)data.upgradeType)}";

            }
            descriptionText.text += $"</b></style>\n\n{data.description}";

            costText.text = node.cost.ToString();

            gameObject.SetActive(true);
        }

        private void handleUpgradeNode() {
            GraphManager.onNodeUpgrade?.Invoke(id);
        }

        private void handleNodeUpgraded(Node node, bool hasEnoughMoney, bool isMaxLevel) {
            levelText.text = $"{int.Parse(levelText.text.Split('/')[0]) + 1}/{node.nodeData.maxLevel}";
            if (isMaxLevel)
            {
                upgradeButton.gameObject.SetActive(false);
                costArea.SetActive(false);
                return;
            }
            if (!hasEnoughMoney) {
                upgradeButton.interactable = false;
            }
            costText.text = node.cost.ToString();
        }

        public void close() {
            anim.SetBool("Exit", true);
        }

        public void destroySelf() {
            Destroy(gameObject);
        }
    }
}