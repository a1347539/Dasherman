using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using System.Linq;
using FYP.Global.WaitingRoom;
using UnityEngine.UI;
using FYP.Global.Photon;
using FYP.Global;

namespace FYP.WaitingRoom
{
    public class UIController : Singleton<UIController>
    {

        [SerializeField]
        private GameObject startGameButton;

        public Action<PhotonTeam> onSwitchTeam = delegate { };


        private List<PlayerCardHolder> playerCardHolders = new List<PlayerCardHolder>();
        public List<PlayerCardHolder> PlayerCardHolders { get { return playerCardHolders; } }

        private void Awake()
        {
            PlayerController.onSwitchPosition += handleSwitchPosition;
            PlayerController.onToggledIsReady += handleToggleIsReady;
            PlayerController.onGameStartable += handleStartGameButtonInteractable;
            RoomController.onJoinRoom += handleCreateAllPlayerCards;
            RoomController.onOtherPlayerLeaveRoom += handleResetOtherPlayerGameObject;
        }

        private void OnDestroy()
        {
            PlayerController.onSwitchPosition -= handleSwitchPosition;
            PlayerController.onToggledIsReady -= handleToggleIsReady;
            PlayerController.onGameStartable -= handleStartGameButtonInteractable;
            RoomController.onJoinRoom -= handleCreateAllPlayerCards;
            RoomController.onOtherPlayerLeaveRoom -= handleResetOtherPlayerGameObject;
        }

        private PlayerCardHolder getPlayerCardHolderByPlayer(Player player) {
            foreach (PlayerCardHolder holder in playerCardHolders)
            {
                if (holder.Card.player == null) { continue; }
                if (holder.Card.player.NickName == player.NickName)
                {
                    return holder;
                }
            }
            return null;
        }

        private void handleSwitchPosition(Player player, int newPosition)
        {
            PlayerCardHolder h = getPlayerCardHolderByPlayer(player);
            if (h != null) { h.reset(); }
            playerCardHolders[newPosition].initializePlayerCard(player, false);
        }

        private void handleToggleIsReady(Player player, Boolean isReady)
        {
            // print("player on ready " + player.NickName);
            PlayerCardHolder h = getPlayerCardHolderByPlayer(player);
            if (h == null) { return; }
            // print("index " + h.index);
            h.toggleIsReady(isReady);
        }

        private void handleStartGameButtonInteractable(bool obj)
        {
            // debug
            startGameButton.GetComponent<Button>().interactable = true;
            return;
            startGameButton.GetComponent<Button>().interactable = obj;
        }

        private void handleResetOtherPlayerGameObject(Player otherPlayer)
        {
            if (otherPlayer != PhotonNetwork.LocalPlayer)
            {
                PlayerCardHolder h = getPlayerCardHolderByPlayer(otherPlayer);
                if (h != null) { h.reset(); }
            }
        }

        private void handleCreateAllPlayerCards(bool isRejoin) {
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++) {
                if (!isRejoin)
                {
                    if (players[i] == PhotonNetwork.LocalPlayer) { break; }
                }
                int cardPosition = (int)NetworkUtilities.getCustomProperty(players[i], SettingKeys.Position);
                Boolean isReady = (Boolean)NetworkUtilities.getCustomProperty(players[i], SettingKeys.IsReady);
                playerCardHolders[cardPosition].initializePlayerCard(players[i], isReady);
            }
        }

        public void configureButtomButtons()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient) {
                startGameButton.SetActive(true);
            }
        }

    }
}
