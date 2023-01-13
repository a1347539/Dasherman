using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using FYP.Global;
using FYP.Global.InGame;
using FYP.InGame.PlayerInstance;
using Photon.Realtime;
using FYP.InGame.Map;
using FYP.InGame.Weapon;
using FYP.InGame.BreakableObject;
using FYP.Global.Photon;
using System.Linq;
using Random = UnityEngine.Random;

namespace FYP.InGame.Photon
{
    public class MapObjectManager : PunSingleton<MapObjectManager>
    {
        [SerializeField]
        public Transform breakableObjectContainer;

        private ScriptableBreakableObject[] breakableObjectDatas;


        [SerializeField]
        public Transform characterContainer;

        public ScriptableCharacter[] charactersDatas { get; private set; }
        public ScriptableWeapon[] weaponDatas { get; private set; }

        public GameObject selfCharacter { get; private set; }


        private void Awake()
        {
            MapController.onMapLoaded += handleSpawnCharacters;
            MapController.onMapLoaded += handlePlaceBreakableObjects;
            GameManager.onExitInGameScene += handleDestroyMapObjects;
        }

        private void OnDestroy()
        {
            MapController.onMapLoaded -= handleSpawnCharacters;
            MapController.onMapLoaded -= handlePlaceBreakableObjects;
            GameManager.onExitInGameScene -= handleDestroyMapObjects;
        }

        public void handleSpawnCharacters()
        {
            charactersDatas = Resources.LoadAll<ScriptableCharacter>(PlayerKeys.scriptableCharacterPathPrefix);
            weaponDatas = Resources.LoadAll<ScriptableWeapon>(WeaponKeys.scriptableWeaponPathPrefix);

            #region debug
            // assume getting player position
            int positionIndex = Random.Range(0, 7);
            int id = 1015;
            #endregion

            //int positionIndex = (int)NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerKeys.Position);
            //int id = (int)NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerGlobalCustomProperties.PlayerClassID);


            ScriptableCharacter sc = charactersDatas.First(data => data.characterId == id);
            ScriptableWeapon sw = weaponDatas.First(data => data.weaponID == sc.defaultWeaponID);

            GameObject weapon = NetworkUtilities.networkInstantiate(sw.weaponPrefab, Vector2.zero, Quaternion.identity, false);
            GameObject character = NetworkUtilities.networkInstantiate(sc.characterPrefab, Vector2.zero, Quaternion.identity, false);
            weapon.GetComponent<WeaponController>().initialize(sw, character.GetPhotonView());
            character.GetComponent<CharacterBuilder>().initialize(positionIndex, sc, weapon, sw.attackType);
            
            PhotonEvents.InGameEvents.notifyCharacterSpawnEvent(character.GetPhotonView().ViewID);
        }

        public void handlePlaceBreakableObjects()
        {
            breakableObjectDatas = Resources.LoadAll<ScriptableBreakableObject>(BreakableObjectKeys.scriptableBreakableObjectPathPrefix);

            List<Map.BreakableObject> objects = MapController.Instance.initPoints.breakableObjectSpawnPoints;

            foreach (var data in breakableObjectDatas) {
                print(data.name);
            }


            for (int i = 0; i < objects.Count; i++)
            {
                foreach (Map.SpawnPoint spawnPoint in objects[i].objectSpawnPoints)
                {
                    GameObject obj = NetworkUtilities.networkInstantiate(breakableObjectDatas[i].prefab, Vector2.zero, Quaternion.identity, true);
                    if (obj != null)
                    {
                        obj.GetComponent<BreakableObject.BreakableObject>().initialize(breakableObjectDatas[i].health, spawnPoint.getPoint());
                    }
                }
            }
        }

        private void handleDestroyMapObjects() {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber, false);
        }

        public void onPlayerDebugButtonClick() {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject obj = (GameObject)PhotonNetwork.PlayerList[i].TagObject;
                PlayerInstance.CharacterController c = obj.GetComponent<PlayerInstance.CharacterController>();
                print(i + " " + c.currentPoint.x + " " + c.currentPoint.y);
            }
        }
    }
}