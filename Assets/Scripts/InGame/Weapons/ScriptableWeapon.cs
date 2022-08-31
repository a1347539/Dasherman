using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace FYP.InGame.Weapon
{
    public enum WeaponAttackType { 
        Slash = 0,
        Thrust = 1,
        Ranged = 2,
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "InGame/Weapon")]
    public class ScriptableWeapon : ScriptableObject
    {
        public static int SIZE = 7;

        [Serializable]
        public class Row
        {
            public bool[] column = new bool[SIZE];
        }

        [SerializeField]
        public int weaponID;

        [SerializeField]
        public GameObject weaponPrefab;

        public float attackSpeed;

        public WeaponAttackType attackType;

        public int damage;

        [HideInInspector]
        public Row[] leftRange = new Row[SIZE];
        [HideInInspector]
        public Row[] upRange = new Row[SIZE];
        [HideInInspector]
        public Row[] rightRange = new Row[SIZE];
        [HideInInspector]
        public Row[] downRange = new Row[SIZE];
    }


    [CustomEditor(typeof(ScriptableWeapon))]
    public class CustomScriptInscpector : Editor
    {

        ScriptableWeapon targetScript;

        void OnEnable()
        {
            targetScript = target as ScriptableWeapon;
        }

        public override void OnInspectorGUI()
        { 
            DrawDefaultInspector();

            {
                GUILayout.Label("Attack range matrixes, for facing right");
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < ScriptableWeapon.SIZE; y++)
                {
                    EditorGUILayout.BeginVertical();
                    for (int x = 0; x < ScriptableWeapon.SIZE; x++)
                    {
                        targetScript.rightRange[x].column[y] = EditorGUILayout.Toggle(targetScript.rightRange[x].column[y]);
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            {
                GUILayout.Label("Attack range matrixes, for facing up");
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < ScriptableWeapon.SIZE; y++)
                {
                    EditorGUILayout.BeginVertical();
                    for (int x = 0; x < ScriptableWeapon.SIZE; x++)
                    {
                        targetScript.upRange[x].column[y] = EditorGUILayout.Toggle(targetScript.upRange[x].column[y]);
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            {
                GUILayout.Label("Attack range matrixes, for facing left");
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < ScriptableWeapon.SIZE; y++)
                {
                    EditorGUILayout.BeginVertical();
                    for (int x = 0; x < ScriptableWeapon.SIZE; x++)
                    {
                        targetScript.leftRange[x].column[y] = EditorGUILayout.Toggle(targetScript.leftRange[x].column[y]);
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            {
                GUILayout.Label("Attack range matrixes, for facing down");
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < ScriptableWeapon.SIZE; y++)
                {
                    EditorGUILayout.BeginVertical();
                    for (int x = 0; x < ScriptableWeapon.SIZE; x++)
                    {
                        targetScript.downRange[x].column[y] = EditorGUILayout.Toggle(targetScript.downRange[x].column[y]);
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
    }
}