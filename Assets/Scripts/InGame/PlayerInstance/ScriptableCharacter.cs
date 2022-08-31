using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerInstance
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "InGame/Character")]
    public class ScriptableCharacter : ScriptableObject
    {
        [SerializeField]
        public GameObject characterPrefab;
        [SerializeField]
        public Sprite classIcon;

        public int defaultWeaponID;

        public string characterName;

        public int characterId;

        public int baseHealth = 10;
        public int baseMana = 10;
        public int baseDashingDamage = 3;
        // per second, need changing to scaling
        public int baseManaRegenRate = 3;

        [Header("class attributes")]
        // up to 10
        public float healthScaling;
        public float manaScaling;
        public float physicalDamageScaling;
        public float magicDamageScaling;
        public float physicalDefenceScaling;
        public float magicDefenceScaling;
        public float manaRegenScaling;
    }
}