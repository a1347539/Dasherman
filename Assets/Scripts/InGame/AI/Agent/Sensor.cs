using FYP.InGame.AI.Environment;
using FYP.InGame.AI.Environment.Character;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FYP.InGame.AI.Agent
{
    public class Sensor : MonoBehaviour
    {
        public List<int> getProcessedTileMap()
        {
            return AIManager.Instance.processedTileMatrix;
        }


        public int getHealth() {
            return GetComponent<CharacterVital>().currentHealth;
        }

        public int getMana() {
            return GetComponent<CharacterVital>().currentMana;
        }

        public Vector2 getSelfPosition() {
            Point p = GetComponent<Environment.Character.CharacterController>().currentPoint;
            return new Vector2(p.x, p.y);
        }
    }
}