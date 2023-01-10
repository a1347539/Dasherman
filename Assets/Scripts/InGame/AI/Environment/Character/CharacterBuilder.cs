using FYP.Global.InGame;
using Photon.Pun;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterBuilder : MonoBehaviour
    {
        // for AI trainning
        public int teamNumber;

        [SerializeField]
        public Transform weaponOffsetTransform;

        [SerializeField]
        private UIManager uiManager;

        [SerializeField]
        private GameObject sprite;

        public GameObject Sprite { get { return sprite; } }

        private CharacterVital vital;
        private CharacterController controller;
        private CharacterAttackController attackController;
        private CharacterAnimationManager animationManager;

        public bool isInitialized = false;

        private void Awake()
        {
            vital = GetComponent<CharacterVital>();
            controller = GetComponent<CharacterController>();
            attackController = GetComponent<CharacterAttackController>();
            animationManager = GetComponent<CharacterAnimationManager>();
        }

        // the first parameter is changed from int positionIndex to Point point for AI training
        // team numbers are either 1 or 2
        public void initialize(Point point, PlayerInstance.ScriptableCharacter sc, GameObject weapon, InGame.Weapon.WeaponAttackType attackType, int teamNumber)
        {
            this.teamNumber = teamNumber;
            uiManager.TeamIndicator.setTeamColor(teamNumber);

            Point p = point;

            transform.SetParent(MapObjectManager.Instance.characterContainer);
            controller.setCurrentPoint(p, false);

            vital.initializeStats(Mathf.FloorToInt(sc.baseHealth * sc.healthScaling),
                Mathf.FloorToInt(sc.baseMana * sc.manaScaling),
                sc.baseManaRegenRate * sc.manaRegenScaling,
                sc.physicalDefenceScaling,
                sc.magicDefenceScaling);

            controller.CharacterState = CharacterController.CharacterStates.idle;
            transform.localScale = MapController.Instance.characterSpriteScaling;

            isInitialized = true;
            attackController.initializeStats(sc.baseDashingDamage, sc.physicalDamageScaling, sc.magicDamageScaling, weapon, attackType);
            animationManager.initializeAnimator();
        }
    }
}