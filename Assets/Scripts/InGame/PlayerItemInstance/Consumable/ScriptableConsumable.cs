using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FYP.InGame.PlayerItemInstance.Consumable
{
    [CreateAssetMenu(fileName = "NewConsumable", menuName = "InGame/PlayerItem/Consumable")]
    public class ScriptableConsumable : ScriptablePlayerItem
    {
        public float healthRecoveryRatio;
        public float manaRecoveryRatio;

        [Header("Scaling are additive")]

        public float damageScaling;
        public float defenceScaling;
        public int duration;
    }
}
