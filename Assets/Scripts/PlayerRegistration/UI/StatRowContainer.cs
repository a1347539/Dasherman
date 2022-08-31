using FYP.InGame.PlayerInstance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FYP.PlayerRegistration
{
    public class StatRowContainer : MonoBehaviour
    {
        [SerializeField]
        private Slider[] sliders;

        private void Start()
        {
            sliders = GetComponentsInChildren<Slider>();
        }

        public void initializeRows(ScriptableCharacter characterData)
        {
            sliders[0].value = characterData.healthScaling/10;
            sliders[1].value = characterData.manaScaling/10;
            sliders[2].value = characterData.physicalDamageScaling/10;
            sliders[3].value = characterData.magicDamageScaling/10;
            sliders[4].value = characterData.physicalDefenceScaling/10;
            sliders[5].value = characterData.magicDefenceScaling/10;
            sliders[6].value = characterData.manaRegenScaling/10;
        }

    }
}