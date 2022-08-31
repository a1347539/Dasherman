using UnityEngine;
using Photon.Pun;

namespace FYP.InGame.PlayerItemInstance
{
    public enum PlayerUsableType { 
        Consumable = 0,
        Skill = 1,
    }

    public enum PlayerUsableTarget { 
        self = 0,
        all = 1,
        random = 2,
    }

    public interface PlayerUsable
    {
        public GameObject IconPrefab { get; }

        public PlayerUsableType playerItemType { get; }
        public byte playerItemID { get; }
        public string playerItemName { get; }
        public string playerItemDescription { get; }
        public PlayerUsableTarget target { get; }

        public void initialize(ScriptablePlayerItem item);

        public void onSelected(PhotonView pv);

        public void useItem(PhotonView user, PhotonView receiver = null);
    }
}