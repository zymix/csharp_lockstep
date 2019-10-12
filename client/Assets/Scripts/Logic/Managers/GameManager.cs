
using System;
using System.Collections.Generic;
using Lockstep.Logic;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    public class GameManager : UnityBaseManager {
        public static GameManager Instance { get; private set; }
        public static PlayerInput CurGameInput = new PlayerInput();

        [Header("ClientMode")] public bool isClientMode;
        public PlayerServerInfo ClientModeInfo = new PlayerServerInfo();

        [Header("Recorder")] public bool isReplay = false;
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
        private List<UnityBaseManager> managers = new List<UnityBaseManager>();

        private static string _traceLogPath {
            get {
#if UNITY_STANDALONE_OSX
                return $"/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#else
                return $"c:/tmp/LPDemo/Dump_{Instance.localPlayerId}.txt";
#endif
            }
        }

        public void RegisterManagers(UnityBaseManager mgr) {
            managers.Add(mgr);
        }

        private void Awake() {
            Screen.SetResolution(1024, 768, false);
            gameObject.AddComponent<PingMono>();
            gameObject.AddComponent(InputMono)();

            _Awake();
        }

        private void Start() {
            _Start();
        }

        private void Update() {
            _DoUpdate();
        }

        private void _Awake() {
#if !UNITY_EDITOR
            IsReplay = false;
#endif
            DoAwake();
            foreach(var mgr in managers) {
                mgr.DoAwake();
            }
        }

        private void _Start() {
            DoStart();
            foreach (var mgr in managers) {
                mgr.DoStart();
            }
            Debug.Trace("Before StartGame _IdCounter:" + BaseEntity.idCounter);
            if(!isReplay && !isClientMode) {
                netClient = new NetClient();
                netClient.Start();
                netClient.Send(new Msg_JoinRoom() { name = Application.dataPath });
            } else {
                StartGame(0, playerServerInfos, localPlayerId);
            }
        }

        private void _DoUpdate() {
            if (!_hasStart) return;
            remainTime += Time.deltaTime;
            while(remainTime >= 0.03f) {
                remainTime -= 0.03f;
                if (!isReplay) {
                    SendInput();
                }
                if(GetFrame(curFrameIdx) == null) {
                    return;
                }

                Step();
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
                Debug.Trace("createPlayer");
                allPlayers.Add(new Player() { localId = i });
            }
            //创建玩家
            for(int i = 0; i < playerCount; ++i) {
                var playerInfo = playerInfos[i];
                var go = HeroManager.InstantiateEntity(allPlayers[i], playerInfo.prefabId, playerInfo.initPos);
                if (allPlayers[i].localId == localPlayerId) {
                    MyPlayerTrans = go.transform;
                }
            }
            MyPlayer = allPlayers[localPlayerId];
        }

        private void Step() {
            UpdateFrameInput();
            if (isReplay) {
                if(curFrameIdx < frames.Count) {
                    Replay(curFrameIdx);
                    ++curFrameIdx;
                }
            } else {
                Recoder();
                netClient?.Send(new Msg_HashCode() { tick = curFrameIdx, hash = GetHash() };
                TraceHelper.TraceFrameState();
                ++curFrameIdx;
            }
        }

        public static void PushFrameInput(FrameInput input) {
            var frames = Instance.frames;
            for(int i = frames.Count; i <= input.tick; ++i) {
                frames.Add(new FrameInput());
            }

            if(frames.Count == 0) {
                Instance.remainTime = 0;
            }
            _maxServerFrameIdx = Math.Max(_maxServerFrameIdx, input.tick);
            if(Instance.tick2SendTimer.TryGetValue(input.tick, out var val)) {
                Delays.Add(Time.realtimeSinceStartup - val);
            }
            frames[input.tick] = input;
        }

        public void SendInput() {
            if (isClientMode) {
                PushFrameInput(new FrameInput() {
                    tick = curFrameIdx,
                    inputs = new PlayerInput[] { CurGameInput }
                });
                return;
            }
            predictTickCount = 2;
            if(inputTick > predictTickCount + _maxServerFrameIdx) {
                return;
            }
            var playerInput = CurGameInput;
            netClient?.Send(new Msg_PlayerInput() { input = playerInput, tick = inputTick });
            tick2SendTimer[inputTick] = Time.realtimeSinceStartup;
            ++inputTick;
        }

        public FrameInput GetFrame(int tick) {
            if(frames.Count > tick) {
                var frame = frames[tick];
                if(frame != null && frame.tick == tick) {
                    return frame;
                }
            }
            return null;
        }
    }
}
