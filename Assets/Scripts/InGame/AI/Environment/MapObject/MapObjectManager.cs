using FYP.Global.InGame;
using FYP.InGame.AI.Environment.Character;
using FYP.InGame.AI.Environment.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FYP.InGame.AI.Environment
{
    public class MapObjectManager : MonoBehaviour
    {
        public Action spawnCharacterAvailable;

        public Action spawnBreakableObjectAvailable;

        [SerializeField]
        private MapController mapController;
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        public Transform breakableObjectContainer;
        private InGame.BreakableObject.ScriptableBreakableObject[] breakableObjectDatas;
        [SerializeField]
        public Transform characterContainer;
        public PlayerInstance.ScriptableCharacter[] charactersDatas { get; private set; }
        public InGame.Weapon.ScriptableWeapon[] weaponDatas { get; private set; }

        private void Awake()
        {
            mapController.onMapLoaded += handleSpawnCharacters;
            mapController.onMapLoaded += handlePlaceBreakableObjects;
        }

        void Start()
        {
            
        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            mapController.onMapLoaded -= handleSpawnCharacters;
            mapController.onMapLoaded -= handlePlaceBreakableObjects;
        }

        private void handleSpawnCharacters()
        {
            charactersDatas = Resources.LoadAll<PlayerInstance.ScriptableCharacter>(AIKeys.scriptableCharacterPathPrefix);
            weaponDatas = Resources.LoadAll<InGame.Weapon.ScriptableWeapon>(AIKeys.scriptableWeaponPathPrefix);

            spawnCharacterAvailable?.Invoke();

        }

        public void handlePlaceBreakableObjects()
        {
            breakableObjectDatas = Resources.LoadAll<InGame.BreakableObject.ScriptableBreakableObject>(AIKeys.AIEnvironmentPathPrefix);
            spawnBreakableObjectAvailable?.Invoke();
        }

        /// <summary>
        /// teamId are either 1 or 2
        /// </summary>
        public GameObject spawnCharacter(int characterId, int teamId) {
            PlayerInstance.ScriptableCharacter sc = charactersDatas.First(data => data.characterId == characterId);
            InGame.Weapon.ScriptableWeapon sw = weaponDatas.First(data => data.weaponID == sc.defaultWeaponID);

            GameObject weapon = Instantiate(sw.weaponPrefab, Vector2.zero, Quaternion.identity);
            GameObject character = Instantiate(sc.characterPrefab, characterContainer);

            weapon.GetComponent<WeaponController>().initialize(sw, character);
            character.GetComponent<CharacterBuilder>().initialize(gameManager.getEmptyPoint(), sc, weapon, sw.attackType, teamId);

            return character;
        }

        public void spawnBreakableObjects(int min, int max) {
            int numberOfBreakableObject = Random.Range(min, max);
            for (int i = 0; i < numberOfBreakableObject; ++i)
            {
                int ObjectIndex = Random.Range(0, breakableObjectDatas.Length - 1);
                GameObject obj = Instantiate(breakableObjectDatas[ObjectIndex].prefab, breakableObjectContainer);
                if (obj != null)
                {
                    obj.GetComponent<BreakableObject>().initialize(breakableObjectDatas[ObjectIndex].health, gameManager.getEmptyPoint());
                }
            }
        }
    }
}