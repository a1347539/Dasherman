using FYP.InGame;
using System.IO;
using UnityEngine;

namespace FYP.Global
{
    public static class SceneName {
        public const string Login = "Login";
        public const string PlayerRegistration = "PlayerRegistration";
        public const string MainLobby = "MainLobby";
        public const string WaitingRoom = "WaitingRoom";
        public const string InGame = "InGame";

        public const string Upgrade = "Upgrade";
    }

    public static class PlayFabKeys 
    {
        public const string PlayerClass = "PlayerClass";
        public const string PlayerLevel = "PlayerLevel";
        public const string PlayerGold = "PlayerGold";
        public const string PlayerExp = "PlayerExp";
        public const string PlayerUpgrades = "PlayersUpgrades";
        public const string PlayerTotalUpgradeInfo = "PlayerTotalUpgradeInfo";


    }

    public static class UserSettingKeys
    {
        public const string Username = "Username";
        public const string GameVersion = "GameVersion";
    }

    public static class PhotonCodes
    {
        public const byte loadSceneEvent = 51;

        public const byte updateTileMatrixEvent = 91;
        public const byte applyDamageToTargetEvent = 92;
        public const byte notifyCharacterSpawnEvent = 93;
        public const byte notifyPlayerOnDieEvent = 94;
        public const byte playerDiedEvent = 95;
        public const byte endGameEvent = 98;

        public const byte notifyPlayerItemSpawn = 109;
        public const byte useItemEvent = 110;

        public const byte pointType = 10;
    }

    public static class GameSettingKeys
    {
        public enum GameModes
        {
            PVP, PVE
        }

        public enum PVPGameModes { 
            Team = 0,
            Solo = 1,
        }

        public const int teamNumberForPVE = 10;
        public const int teamNumberForPVP = 2;

        public const string GameMode = "GameMode";
        public const GameModes defaultGameMode = GameModes.PVP;
        public const int defaultRoomCapacity = 6;
    }

    public static class PlayerItemID
    {
        public enum ConsumableID
        {
            HealingPotion = 10,
            ManaPotion = 11,

            GlobalDamagePotion = 23

        }

        public enum SkillID
        {
            Explosion = 10,
            Lightning = 11,
            IcePillar = 12,
        }
    }

    public static class PlayerGlobalCustomProperties
    {
        public const string PlayerClassID = "PlayerClassID";
    }

    namespace WaitingRoom
    {
        public static class SettingKeys
        {
            public const string IsReady = "IsReady";
            public const string Position = "Position";
            public const int InitPosition = -1;
            public const string TeamNumber = "TeamNumber";
        }
    }

    namespace InGame 
    {
        public static class MapKeys 
        {
            public static string mapReachableTag = "map.reachable";
            public static string mapUnreachableTag = "map.unreachable";
            public static string scriptableMapPathPrefix = "InGame";
            public static string networkObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "Map";
        }

        public static class PlayerKeys 
        {
            public const string Position = "Position";
            public const string TeamNumber = "TeamNumber";
            public const string IsCharacterSpawned = "IsCharacterSpawned";

            public static string InGameScore = "InGameScore";

            public static string scriptableCharacterPathPrefix = "InGame" + Path.DirectorySeparatorChar + "PlayerInstance";
            public static string networkObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "PlayerInstance";
        }

        public static class AIKeys 
        {
            public static int AITeamNumber = 101;

            public static string AIEnvironmentPathPrefix = "AI" + Path.DirectorySeparatorChar + "Environment";

            public static string scriptableCharacterPathPrefix = "AI" + Path.DirectorySeparatorChar + "Agent";
            public static string scriptableWeaponPathPrefix = "AI" + Path.DirectorySeparatorChar + "Weapon";
        }

        public static class PlayerItemKeys 
        {

            public const int NumberOfConsumables = 2;
            public const int NumberOfSkills = 3;
            public const int NumberbOfSlot = 5;

            public const string PlayerConsumableIDs = "PlayerConsumableIDs";
            public const string PlayerConsumableAmounts = "PlayerConsumableAmounts";
            public const string PlayerSkillIDs = "PlayerSkillIDs";
            public const string PlayerSkillAmounts = "PlayerSkillAmount";

            public static string scriptablePlayerItemPathPrefix = "InGame" + Path.DirectorySeparatorChar + "PlayerItemInstance";
            public static string networkObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "PlayerItemInstance";
        }

        public static class CharacterKeys 
        {
            public const float deathAnimationSpeed = 1f;
            public static Vector2 DeadCharacterPosition = new Vector2(100, 100);
        }

        public static class WeaponKeys {
            public static string scriptableWeaponPathPrefix = "InGame" + Path.DirectorySeparatorChar + "Weapon";
            public static string networkObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "Weapon";
        }

        public static class BreakableObjectKeys {
            public static string scriptableBreakableObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "MapObject";
            public static string networkObjectPathPrefix = "InGame" + Path.DirectorySeparatorChar + "MapObject";
            public static Vector2 DeadObjectPosition = new Vector2(110, 100);
        }

        public static class UIKeys {
            public static string CanvasContainerTag = "CanvasContainer";
            public const string EndGameTimerMessage = "Heading back to the waiting room in";
        }
    }
    namespace Upgrade
    {
        public static class GraphKeys
        {
            public const string resourceObjectPathPrefix = "Upgrade";
        }
    }
}
