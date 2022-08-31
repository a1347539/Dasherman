using FYP.Global;
using FYP.Global.InGame;
using FYP.Global.Photon;
using FYP.InGame.Map;
using FYP.InGame.PlayerInstance;
using FYP.InGame.PlayerItemInstance;
using FYP.InGame.Weapon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FYP.InGame.AI
{
    public class AIManager : MonoBehaviour
    {
        public ScriptableCharacter[] charactersDatas { get; private set; }
        public ScriptableWeapon[] weaponDatas { get; private set; }
        public ScriptablePlayerItem[] playerItemDatas { get; private set; }

        private bool AISpawned = false;

        private void Awake()
        {
            MapController.onMapLoaded += handleSpawnAIEnemies;
        }

        private void OnDestroy()
        {
            MapController.onMapLoaded -= handleSpawnAIEnemies;
        }

        private void createItems(List<ScriptablePlayerItem> items)
        {
            foreach (ScriptablePlayerItem item in items)
            {

            }
        }

        private void handleSpawnAIEnemies() {
            if (PhotonNetwork.LocalPlayer.IsMasterClient && !AISpawned)
            {
                charactersDatas = Resources.LoadAll<ScriptableCharacter>(PlayerKeys.scriptableCharacterPathPrefix);
                weaponDatas = Resources.LoadAll<ScriptableWeapon>(WeaponKeys.scriptableWeaponPathPrefix);
                playerItemDatas = Resources.LoadAll<ScriptablePlayerItem>(PlayerItemKeys.scriptablePlayerItemPathPrefix);

                createNewEnemy();
                AISpawned = true;
            }
        }

        private void createNewEnemy() {
            print("creating AI");
            int positionIndex = Random.Range(0, 7);
            ScriptableCharacter sc = charactersDatas.First(data => data.characterId == 1017);
            ScriptableWeapon sw = weaponDatas.First(data => data.weaponID == sc.defaultWeaponID);

            GameObject weapon = NetworkUtilities.networkInstantiate(sw.weaponPrefab, Vector2.zero, Quaternion.identity, true);
            GameObject character = NetworkUtilities.networkInstantiate(sc.characterPrefab, Vector2.zero, Quaternion.identity, true);
            weapon.GetComponent<WeaponController>().initialize(sw, character.GetPhotonView());
            character.GetComponent<CharacterBuilder>().initialize(positionIndex, sc, weapon, sw.attackType);
            character.AddComponent<AIInput>();
            PhotonEvents.InGameEvents.notifyCharacterSpawnEvent(character.GetPhotonView().ViewID, true);
        }
    }
}