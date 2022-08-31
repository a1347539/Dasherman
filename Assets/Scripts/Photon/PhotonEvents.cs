using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FYP.Global.InGame;
using ExitGames.Client.Photon;
using FYP.InGame.Map;

namespace FYP.Global.Photon
{
    public class PhotonEvents
    {
        public class WaitingRoomEvents { 
            
        }

        public class InGameEvents
        {
            public static void updateTileMatrixEvent(Point p, bool isEntry, int photonViewID)
            {
                object[] content = new object[] { p, isEntry, photonViewID };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PhotonCodes.updateTileMatrixEvent, content, options, SendOptions.SendReliable);
            }

            public static void notifyCharacterSpawnEvent(int photonViewID, bool isNPC = false)
            {
                object[] content = new object[] { photonViewID, isNPC };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PhotonCodes.notifyCharacterSpawnEvent, content, options, SendOptions.SendReliable);
            }

            public static void useItemEvent(byte itemType, byte itemID, byte itemTarget, int userPVID, int? receiverPVID) {
                object[] content = new object[] { itemType, itemID, itemTarget, userPVID, receiverPVID };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PhotonCodes.useItemEvent, content, options, SendOptions.SendReliable);
            }

            public static void playerDiedEvent()
            {
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                PhotonNetwork.RaiseEvent(PhotonCodes.playerDiedEvent, new object[] {}, options, SendOptions.SendReliable);
            }

            public static void endGameEvent()
            {
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(PhotonCodes.endGameEvent, new object[0], options, SendOptions.SendReliable);
            }
        }

        public static void loadSceneEvent(string fromScene, string toScene) {
            object[] content = new object[] { fromScene, toScene };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonCodes.loadSceneEvent, content, options, SendOptions.SendReliable);
        }
    }
}