using FYP.Global.InGame;
using FYP.InGame.Photon;
using FYP.InGame.PlayerItemInstance.Consumable;
using FYP.InGame.PlayerItemInstance.Skill;
using FYP.InGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using FYP.Global;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace FYP.InGame.PlayerItemInstance
{
    public class PlayerItemManager : Singleton<PlayerItemManager>
    {
        public class PlayerItemAmountPair {
            public ScriptablePlayerItem playeritem;
            public int amount;
            public PlayerItemAmountPair(ScriptablePlayerItem i, int a) {
                playeritem = i;
                amount = a;
            }
        }

        [SerializeField]
        private GameObject itemWheelCanvasPrefab;
        public ItemWheel itemWheel { get; private set; }

        private ScriptablePlayerItem[] playerItemDatas;

        [SerializeField]
        public Transform globalPlayerItemContainer;
        public List<PlayerUsable> globalPlayerItems { get; private set; } = new List<PlayerUsable>();
        public List<PlayerUsable> localPlayerItems { get; private set; } = new List<PlayerUsable>();

        public List<PlayerItemAmountPair> playerItemAmountPairs = new List<PlayerItemAmountPair>();

        public int numberOfSlots { get { return PlayerItemKeys.NumberbOfSlot; } }
        public int numberOfConsumables { get { return PlayerItemKeys.NumberOfConsumables; } }
        public int numberOfSkills { get { return PlayerItemKeys.NumberOfSkills; } }

        private void Awake()
        {
            PlayerManager.onPlayerItemLoaded += handlePlayerItemLoaded;
        }

        private void OnDestroy()
        {
            PlayerManager.onPlayerItemLoaded -= handlePlayerItemLoaded;
        }

        public void onItemSelected(int slotNumber)
        {
            PlayerInstance.CharacterController.CharacterStates state =
                ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerInstance.CharacterController>().CharacterState;
            if (state != PlayerInstance.CharacterController.CharacterStates.idle && state != PlayerInstance.CharacterController.CharacterStates.aiming)
            {
                return;
            }
            /*globalPlayerItems.First(
                i => {
                    return i.playerItemID == playerItems[slotNumber].playerItemID && i.playerItemType == playerItems[slotNumber].playerItemType; 
                }
                ).onSelected();*/
            localPlayerItems[slotNumber].onSelected(((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetPhotonView());
            itemWheel.disableAllSlots();
        }

        private void handlePlayerItemLoaded()
        {
            playerItemDatas = Resources.LoadAll<ScriptablePlayerItem>(PlayerItemKeys.scriptablePlayerItemPathPrefix);
            UIController.Instance.itemWheelCanvas = Instantiate(itemWheelCanvasPrefab, GameObject.FindGameObjectWithTag(UIKeys.CanvasContainerTag).transform);

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                print(player.NickName);
                List<byte> consumableIDs = new List<byte>((byte[])NetworkUtilities.getCustomProperty(player, PlayerItemKeys.PlayerConsumableIDs));
                List<byte> skillIDs = new List<byte>((byte[])NetworkUtilities.getCustomProperty(player, PlayerItemKeys.PlayerSkillIDs));
                List<ScriptablePlayerItem> playerItems = new List<ScriptablePlayerItem>();
                playerItems = consumableIDs.ConvertAll(id => {
                    if (id == 0) return null;
                    return Array.Find(playerItemDatas, data => data.playerItemID == id && data.playerItemType == PlayerUsableType.Consumable);
                });

                playerItems.AddRange(skillIDs.ConvertAll(id => {
                    if (id == 0) return null;
                    return Array.Find(playerItemDatas, data => data.playerItemID == id && data.playerItemType == PlayerUsableType.Skill);
                }));

                foreach (ScriptablePlayerItem item in playerItems)
                {
                    if (item == null) continue;
                    if (!globalPlayerItems.Exists(i => i.playerItemID == item.playerItemID && i.playerItemType == item.playerItemType))
                    {
                        GameObject newItem = Instantiate(item.ItemPrefab);
                        newItem.GetComponent<PlayerUsable>().initialize(item);
                        globalPlayerItems.Add(newItem.GetComponent<PlayerUsable>());

                    }
                }

                if (player == PhotonNetwork.LocalPlayer)
                {
                    List<byte> consumableAmounts = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerConsumableAmounts));
                    List<byte> skillAmounts = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerSkillAmounts));

                    int i = 0;

                    for (; i < numberOfConsumables; ++i)
                    {
                        playerItemAmountPairs.Add(new PlayerItemAmountPair(playerItems[i], consumableAmounts[i]));
                    }
                    for (int j = 0; i < playerItems.Count; ++i, ++j)
                    {
                        playerItemAmountPairs.Add(new PlayerItemAmountPair(playerItems[i], skillAmounts[j]));
                    }

                    itemWheel = UIController.Instance.itemWheelCanvas.transform.GetChild(0).GetComponent<ItemWheel>();

                    itemWheel.initialize(
                        playerItemAmountPairs.Select(pair => pair.amount).ToList(),
                        playerItemAmountPairs.Select(pair => { if (pair.playeritem == null) return null; return pair.playeritem.IconPrefab; }).ToList()
                        );
                }
            }


            /*
                        List<byte> consumableIDs = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerConsumableIDs));
                        List<byte> consumableAmounts = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerConsumableAmounts));
                        List<byte> skillIDs = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerSkillIDs));
                        List<byte> skillAmounts = new List<byte>((byte[])NetworkUtilities.getCustomProperty(PhotonNetwork.LocalPlayer, PlayerItemKeys.PlayerSkillAmounts));

                        List<ScriptablePlayerItem> playerItems = new List<ScriptablePlayerItem>();
                        playerItems = consumableIDs.ConvertAll(id => {
                            if (id == 0) return null;
                            return Array.Find(playerItemDatas, data => data.playerItemID == id && data.playerItemType == PlayerUsableType.Consumable);
                        });*/

            /*            playerItems.AddRange(skillIDs.ConvertAll(id => {
                            if (id == 0) return null;
                            return Array.Find(playerItemDatas, data => data.playerItemID == id && data.playerItemType == PlayerUsableType.Skill);
                        }));

                        int i = 0;

                        for (; i < numberOfConsumables; ++i)
                        {
                            playerItemPairs.Add(new PlayerItemPair(playerItems[i], consumableAmounts[i]));
                        }
                        for (int j = 0; i < playerItems.Count; ++i, ++j)
                        {
                            playerItemPairs.Add(new PlayerItemPair(playerItems[i], skillAmounts[j]));
                        }

                        itemWheel = UIController.Instance.itemWheelCanvas.transform.GetChild(0).GetComponent<ItemWheel>();

                        itemWheel.initialize(
                            playerItemPairs.Select(pair => pair.amount).ToList(),
                            playerItemPairs.Select(pair => { if (pair.playeritem == null) return null; return pair.playeritem.IconPrefab; }).ToList()
                            );

                        createItems(playerItems);*/
        }
    }
}