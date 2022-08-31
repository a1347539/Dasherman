using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerItemInstance
{
    public class ScriptablePlayerItem : ScriptableObject
    {
        [SerializeField]
        private GameObject itemPrefab;
        public GameObject ItemPrefab { get { return itemPrefab; } }
        public GameObject IconPrefab
        {
            get
            {
                GameObject iconPrefab = itemPrefab.GetComponent<PlayerUsable>().IconPrefab;
                iconPrefab.transform.localScale = new Vector3(0.65f, 0.65f, 1);
                return iconPrefab;
            }
        }

        public PlayerUsableType playerItemType;

        public byte playerItemID;
        public string playerItemName;
        public string playerItemDescription;

        public PlayerUsableTarget target;
    }
}