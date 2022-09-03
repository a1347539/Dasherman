using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Global
{
    public class GlobalMathFunctions
    {
        public static float expByLevel(int level) {
            return Mathf.Pow(level, 3) + 500;
        }

        public static float upgradeCostByUpgradeLevel(int baseCost, int level) {
            return baseCost * (Mathf.Log(level+1) + 1);
        }

        public static float upgradeValueByUpgradeLevel(int baseValue, int level) { 
            return (float)Math.Round(baseValue * (Mathf.Log(level + 1) + 1), 2);
        }
    }
}