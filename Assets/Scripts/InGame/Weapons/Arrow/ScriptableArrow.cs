using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.Weapon
{
    [CreateAssetMenu(fileName = "NewArrow", menuName = "InGame/Arrow")]
    public class ScriptableArrow : ScriptableObject
    {
        public GameObject arrowPrefab;
        public int damage;
        public float speed;
    }
}