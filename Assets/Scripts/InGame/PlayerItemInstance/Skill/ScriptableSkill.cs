using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerItemInstance.Skill
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "InGame/PlayerItem/Skill")]
    public class ScriptableSkill : ScriptablePlayerItem
    {
        public int damage;
        public int manaCost;
        public int cooldownTime;
    }
}