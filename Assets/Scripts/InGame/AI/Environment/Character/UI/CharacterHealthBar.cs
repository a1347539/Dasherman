using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterHealthBar : MonoBehaviour
    {
        [SerializeField]
        private Slider healthBar;

        public void setHealth(float health)
        {
            healthBar.value = health;
        }
    }
}