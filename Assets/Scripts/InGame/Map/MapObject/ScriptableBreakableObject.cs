using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.BreakableObject
{
    [CreateAssetMenu(fileName = "NewMap", menuName = "InGame/BreakableObject")]
    public class ScriptableBreakableObject : ScriptableObject
    {
        public GameObject prefab;
        public int health;
    }
}