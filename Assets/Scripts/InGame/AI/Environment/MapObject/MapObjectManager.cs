using FYP.Global.InGame;
using FYP.InGame.AI.Environment.Character;
using FYP.InGame.AI.Environment.Weapon;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FYP.InGame.AI.Environment
{
    public class MapObjectManager : Singleton<MapObjectManager>
    {
        public Action spawnCharacterAvailable;

        [SerializeField]
        public Transform breakableObjectContainer;
        private InGame.BreakableObject.ScriptableBreakableObject[] breakableObjectDatas;
        [SerializeField]
        public Transform characterContainer;
        public PlayerInstance.ScriptableCharacter[] charactersDatas { get; private set; }
        public InGame.Weapon.ScriptableWeapon[] weaponDatas { get; private set; }

        private void Awake()
        {
            MapController.onMapLoaded += handleSpawnCharacters;
            MapController.onMapLoaded += handlePlaceBreakableObjects;
        }

        void Start()
        {
            
        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            MapController.onMapLoaded -= handleSpawnCharacters;
            MapController.onMapLoaded -= handlePlaceBreakableObjects;
        }

        private void handleSpawnCharacters()
        {
            charactersDatas = Resources.LoadAll<PlayerInstance.ScriptableCharacter>(AIKeys.scriptableCharacterPathPrefix);
            weaponDatas = Resources.LoadAll<InGame.Weapon.ScriptableWeapon>(AIKeys.scriptableWeaponPathPrefix);

            spawnCharacterAvailable?.Invoke();
            // spawnCharacter(1115, 2);
        }

        public void handlePlaceBreakableObjects()
        {
            breakableObjectDatas = Resources.LoadAll<InGame.BreakableObject.ScriptableBreakableObject>(AIKeys.AIEnvironmentPathPrefix);

            int numberOfBreakableObject = Random.Range(15, 40);
            for (int i = 0; i < numberOfBreakableObject; ++i) {
                int ObjectIndex = Random.Range(0, breakableObjectDatas.Length-1);
                GameObject obj = Instantiate(breakableObjectDatas[ObjectIndex].prefab, Vector2.zero, Quaternion.identity);
                if (obj != null)
                {
                    obj.GetComponent<BreakableObject>().initialize(breakableObjectDatas[ObjectIndex].health, GameManager.Instance.getEmptyPoint());
                }
            }
        }

        /// <summary>
        /// teamId are either 1 or 2
        /// </summary>
        public void spawnCharacter(int characterId, int teamId) {
            PlayerInstance.ScriptableCharacter sc = charactersDatas.First(data => data.characterId == characterId);
            InGame.Weapon.ScriptableWeapon sw = weaponDatas.First(data => data.weaponID == sc.defaultWeaponID);

            GameObject weapon = Instantiate(sw.weaponPrefab, Vector2.zero, Quaternion.identity);
            GameObject character = Instantiate(sc.characterPrefab, Vector2.zero, Quaternion.identity);

            weapon.GetComponent<WeaponController>().initialize(sw, character);
            character.GetComponent<CharacterBuilder>().initialize(GameManager.Instance.getEmptyPoint(), sc, weapon, sw.attackType, teamId);
        }
    }
}