using FYP.Global.InGame;
using FYP.InGame.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterBuilder : MonoBehaviour
    {
        public MapController mapController;
        public AIManager aiManager;
        public GameManager gameManager;
        public MapObjectManager mapObjectManager;

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
            mapController = transform.parent.parent.GetComponent<Containers>().mapController;
            aiManager = transform.parent.parent.GetComponent<Containers>().aiManager;
            gameManager = transform.parent.parent.GetComponent<Containers>().gameManager;
            mapObjectManager = transform.parent.parent.GetComponent<Containers>().mapObjectManager;

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
            controller.setCurrentPoint(p, false);

            vital.initializeStats(Mathf.FloorToInt(sc.baseHealth * sc.healthScaling),
                Mathf.FloorToInt(sc.baseMana * sc.manaScaling),
                sc.baseManaRegenRate * sc.manaRegenScaling,
                sc.physicalDefenceScaling,
                sc.magicDefenceScaling);

            controller.CharacterState = CharacterController.CharacterStates.idle;
            transform.localScale = mapController.characterSpriteScaling;

            isInitialized = true;
            attackController.initializeStats(sc.baseDashingDamage, sc.physicalDamageScaling, sc.magicDamageScaling, weapon, attackType);
            animationManager.initializeAnimator();
        }
    }
}