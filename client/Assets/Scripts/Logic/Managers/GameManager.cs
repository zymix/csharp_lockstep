
using System;
using System.Collections.Generic;
using Lockstep.Logic;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class GameManager : UnityBaseManager {
        public static GameManager Instance { get; private set; }
        public static PlayerInput CurGameInput = new PlayerInput();

        [Header("ClientMode")] public bool IsClientMode;
        public PlayerServerInfo ClientModeInfo = new PlayerServerInfo();

        [Header("Recorder")] public bool IsReplay = false;
        public string recordFilePath;

        private static int _maxServerFrameIdx;
        [Header("FrameData")] public int mapId;
        private bool _hasStart = false;
        [HideInInspector] public int predictTickCount = 3;
        [HideInInspector] public int inputTick;
        [HideInInspector] public int localPlayerId = 0;
        [HideInInspector] public int playerCount = 1;
        [HideInInspector] public int curMapId = 0;
        public int curFrameIdx = 0;
        [HideInInspector] public FrameInput curFrameInput;
        [HideInInspector] public PlayerServerInfo[] playerServerInfos;
        [HideInInspector] public List<FrameInput> frames = new List<FrameInput>();

        [Header("Ping")] public static int PingVal;
        public static List<float> Delays = new List<float>();
        public Dictionary<int, float> tick2SendTimer = new Dictionary<int, float>();

        [Header("GameData")] public static List<Player> allPlayers = new List<Player>();
        public static Player MyPlayer;
        public static Transform MyPlayerTrans;
        [HideInInspector] public float remainTime; // remain time to update
        private NetClient netClient;
        private List<UnityBaseManager> _mgrs = new List<UnityBaseManager>();

        private static string _traceLogPath {
            get {
#if UNITY_STANDALONE_OSX
                return $"/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#else
                return $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            }
        }

        public static void StartGame(Msg_StartGame msg) {
            UnityEngine.Debug.Log("StartGame");
            Instance.StartGame(msg.mapId, msg.playerInfos, msg.localPlayerId);
        }

        public void StartGame(int mapId, PlayerServerInfo[] playerInfos, int playerId) {
            _hasStart = true;
            curMapId = mapId;
            playerCount = playerInfos.Length;
            playerServerInfos = playerInfos;
            localPlayerId = playerId;
            Debug.TraceSavePath = _traceLogPath;
            allPlayers.Clear();
            for(int i = 0; i < playerCount; ++i) {
                Debug.Trace("craetePlayer");
                allPlayers.Add(new Player() { localId = i });
            }

            for(int i = 0; i < playerCount; ++i) {
                var playerInfo = playerInfos[i];
                var go = HeroManager.InstantiateEntity(allPlayers[i], playerInfo.prefabId, playerInfo.initPos);
                if (allPlayers[i].localId == localPlayerId) {
                    MyPlayerTrans = go.transform;
                }
            }
            MyPlayer = allPlayers[localPlayerId];
        }

        internal static void PushFrameInput(FrameInput input) {
            throw new NotImplementedException();
        }
    }
}
