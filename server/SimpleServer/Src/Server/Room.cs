using System;
using System.Collections.Generic;
using System.Text;
using Lockstep.Logging;
using Lockstep.Network;

namespace Lockstep.FakeServer{
    class Room {
        public const int MaxPlayerCount = 2;
        private PlayerServerInfo[] playerInfos;
        private Session[] sessions;
        private Dictionary<int, int> id2LocalId = new Dictionary<int, int>();
        private Dictionary<int, PlayerInput[]> tick2Inputs = new Dictionary<int, PlayerInput[]>();
        private Dictionary<int, int[]> tick2Hashes = new Dictionary<int, int[]>();

        private int curLocalId;
        private int curTick;
        private bool IsRunning;

        public void Init(int v) {
            playerInfos = new PlayerServerInfo[MaxPlayerCount];
            sessions = new Session[MaxPlayerCount];
        }

        public DoUpdate(float timeSinceStartUp, float deltaTime) {
            if (!IsRunning) return;
            CheckInput();
        }

        private void CheckInput() {
            if(tick2Inputs.TryGetValue(curTick, out var inputs)) {
                if (inputs != null) {
                    bool isFullInput = true;
                    for(int i = 0; i < inputs.Length; ++i) {
                        if(inputs[i] == null) {
                            isFullInput = false;
                            break;
                        }
                    }
                    if (isFullInput) {
                        BoardInputMsg(curTick, inputs);
                        tick2Inputs.Remove(curTick);
                        curTick++;
                    }
                }
            }
        }

        private void BoardInputMsg(int tick, PlayerInput[] inputs) {
            var frame = new Msg_FrameInput();
            frame.input = new FrameInput() {
                tick = tick,
                inputs = inputs
            };
            var bytes = frame.ToBytes();
            for(int i = 0; i < MaxPlayerCount; ++i) {
                var s = sessions[i];
                sessions.Send((int)EMsgType.FrameInput, bytes);
            }
        }

        public void OnPlayerJoin(Session session, PlayerServerInfo player) {
            if (id2LocalId.ContainsKey(player.id)) return;
            id2LocalId[player.id] = curLocalId;
            playerInfos[curLocalId] = player;
            sessions[curLocalId] = session;
            curLocalId++;
        }

        public void OnPlayerInput(int useId, Msg_PlayerInput msg) {
            int localId = 0;
            if (!id2LocalId.TryGetValue(useId, out localId)) return;
            PlayerInput[] inputs;
            if (!tick2Inputs.TryGetValue(msg.tick, out inputs)) {
                inputs = new PlayerInput[MaxPlayerCount];
                tick2Inputs.Add(msg.tick, inputs);
            }

            inputs[localId] = msg.input;
            CheckInput();
        }

        public void OnPlayerHashCode(int useId, Msg_HashCode msg) {
            int localId = 0;
            if (!id2LocalId.TryGetValue(useId, out localId)) return;
            int[] hashes;
            if (!tick2Hashes.TryGetValue(msg.tick, out hashes)) {
                hashes = new int[MaxPlayerCount];
                tick2Hashes.Add(msg.tick, hashes);
            }

            hashes[localId] = msg.hash;
            //check hash
            foreach (var hash in hashes) {
                if (hash == 0)
                    return;
            }

            bool isSame = true;
            var val = hashes[0];
            foreach (var hash in hashes) {
                if (hash != val) {
                    isSame = false;
                    break;
                }
            }

            if (!isSame) {
                Debug.Log(msg.tick + " Hash is different " + val);
            }

            tick2Hashes.Remove(msg.tick);
        }

        public void OnGameStart() {
            IsRunning = true;
            curTick = 0;
            var frame = new Msg_StartGame();
            frame.mapId = 0;
            frame.playerInfos = playerInfos;
            for (int i = 0; i < MaxPlayerCount; i++) {
                var session = sessions[i];
                frame.localPlayerId = i;
                var bytes = frame.ToBytes();
                session.Send((int)EMsgType.StartGame, bytes);
            }
        }
    }
}
