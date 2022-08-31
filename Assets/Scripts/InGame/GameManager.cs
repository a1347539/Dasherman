using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Global;
using Photon.Pun;
using System;
using Photon.Realtime;
using FYP.Global.InGame;
using ExitGames.Client.Photon;
using FYP.InGame.Photon;
using FYP.InGame.PlayerInstance;
using System.Linq;
using UnityEngine.UI;
using FYP.Global.Photon;
using FYP.InGame.PlayerItemInstance;

namespace FYP.InGame
{
    public class GameManager : Singleton<GameManager>
    {
        public enum GameStates { 
            onGoing = 0,
            end = 1,
        }

        public static Action onSelfDie = delegate { };
        public static Action onGameSetup = delegate { };
        public static Action onEndGame = delegate { };

        public static Action<int> onCharacterCreated = delegate { };
        public static Action<int, bool> onOtherCharacterCreated = delegate { };

        public static Action onExitInGameScene = delegate { };

        private GameStates gameState;

        public GameStates GameState { get { return gameState; }
            set {
                onChangeGameState(gameState, value);
            }
        }

        private void Awake()
        {
            RoomController.onJoinGame += handleJoinGame;
            RoomController.onOtherPlayerLeft += handleCheckEndGameCondition;
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
            onSelfDie += handleRestrictInput;
            onEndGame += handleRestrictInput;
            UI.EndGameCanvas.CountDownTimer.onTimerDone += handleGoToWaitingRoom; 
        }

        private void OnDestroy()
        {
            RoomController.onJoinGame -= handleJoinGame;
            RoomController.onOtherPlayerLeft += handleCheckEndGameCondition;
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
            onSelfDie -= handleRestrictInput;
            onEndGame -= handleRestrictInput;
            UI.EndGameCanvas.CountDownTimer.onTimerDone -= handleGoToWaitingRoom;
        }

        private void onChangeGameState(GameStates oldState, GameStates newState) {

            gameState = newState;
            
            if (gameState == GameStates.onGoing) {
                print("game start");
                onGameSetup?.Invoke();
            }
            if (gameState == GameStates.end) {
                print("game end");
                onEndGame?.Invoke();
            }
        }

        private void handleJoinGame()
        {
            NetworkUtilities.loadNetworkPrefabs(MapKeys.networkObjectPathPrefix);
            NetworkUtilities.loadNetworkPrefabs(PlayerKeys.networkObjectPathPrefix);
            NetworkUtilities.loadNetworkPrefabs(WeaponKeys.networkObjectPathPrefix);
            NetworkUtilities.loadNetworkPrefabs(BreakableObjectKeys.networkObjectPathPrefix);
            NetworkUtilities.loadNetworkPrefabs(PlayerItemKeys.networkObjectPathPrefix);
            GameState = GameStates.onGoing;
        }

        private void handleRestrictInput() {
            InputManager.Instance.enabled = false;
            UIController.Instance.joyStickCanvas.SetActive(false);
        }

        private void handleCheckEndGameCondition()
        {
            // master client check if end game conditions are met, if so, raise end game event to all players.
            // if all alive players are in the same team
            print("check end game condition");

            int teamLeftCount = 0;
            List<InGameTeam> teams = InGameTeamManager.Instance.getAllTeams();
            foreach (InGameTeam team in teams) {
                if (team.views.Count(view =>
                view.gameObject.GetComponent<PlayerInstance.CharacterController>().CharacterState != PlayerInstance.CharacterController.CharacterStates.died) > 0) {
                    ++teamLeftCount;
                }
            }

            print(teamLeftCount);
            if (teamLeftCount > 1) return;
            PhotonEvents.InGameEvents.endGameEvent();
        }

        private void handleGoToWaitingRoom() {
            onExitInGameScene?.Invoke();
            SceneController.LoadScene(SceneName.InGame, SceneName.WaitingRoom);
        }

        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == PhotonCodes.notifyCharacterSpawnEvent) {
                object[] contents = (object[])obj.CustomData;
                print((int)contents[0]);
                onCharacterCreated?.Invoke((int)contents[0]);
            }
            else if (obj.Code == PhotonCodes.playerDiedEvent) {
                object[] contents = (object[])obj.CustomData;
                handleCheckEndGameCondition();
            }
            else if (obj.Code == PhotonCodes.endGameEvent)
            {
                GameState = GameStates.end;
            }
            else if (obj.Code == PhotonCodes.loadSceneEvent)
            {
                object[] contents = (object[])obj.CustomData;
                SceneController.LoadScene((string)contents[0], (string)contents[1]);
            }
            else if (obj.Code == PhotonCodes.useItemEvent)
            {
                // byte itemType, int itemID, byte itemTarget, int userPVID, int? receiverPVID
                object[] contents = (object[])obj.CustomData;
                foreach (PlayerUsable usable in PlayerItemManager.Instance.localPlayerItems)
                {
                    if (usable.playerItemType == (PlayerUsableType)(byte)contents[0] &&
                        usable.playerItemID == (byte)contents[1])
                    {
                        PhotonView user = PhotonView.Find((int)contents[3]);
                        PhotonView receiver = null;
                        if (contents[4] != null)
                        {
                            receiver = PhotonView.Find((int)contents[4]);
                        }
                        usable.useItem(user, receiver);
                    }
                }
            }
        }

        public void onEndGameDebugButtonClick() {
            PhotonEvents.InGameEvents.endGameEvent();
        }
    }
}