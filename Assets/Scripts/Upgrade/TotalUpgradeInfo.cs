using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Upgrade
{
    public class TotalUpgradeInfo
    {
        public List<float> stats;
        public List<int> unlockedSkillId;

        public TotalUpgradeInfo()
        {
            stats = new List<float>() { 0, 0, 0, 0, 0, 0, 0 };
            unlockedSkillId = new List<int>();
        }
    }
}