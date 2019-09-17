using System;
using System.Collections.Generic;
using System.Net;
using Lockstep.Logging;
using Lockstep.Network;
using Lockstep.Util;

namespace Lockstep.FakeServer{
    public class Server:IMessageDispatcher{
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);
        private NetOuterProxy _netProxy = new NetOuterProxy();

        //update
        private const double UpdateInterval = 0.015; //Fame rate = 30
        private DateTime _lastUpdateTimeStamp;
        private DateTime _startUpTimeStamp;
        private double _deltaTime;
        private double _timeSinceStartUp;
        
        //user mgr
        private Room _room;
        private Dictionary<int, PlayerServerInfo> _id2Player = new Dictionary<int, PlayerServerInfo>();
        private Dictionary<int, Session> _id2Session = new Dictionary<int, Session>();
        private Dictionary<String, PlayerServerInfo> _name2Player = new Dictionary<String, PlayerServerInfo>();

        //id
        private static int idCounter = 0;
        private int curCount = 0;

        public void Start(){
            _netProxy.MessageDispatcher = this;
            _netProxy.MessagePacker = MessagePacker.Instance;
            _netProxy.Awake(NetworkProtocol.TCP, serverIpPoint);
            _startUpTimeStamp = _lastUpdateTimeStamp = DateTime.Now;
        }

        public void Dispatch(Session session, Packet packet){
            ushort opcode = packet.Opcode();
            var message = session.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes, Packet.Index,
                packet.Length - Packet.Index) as IMessage;
            //var msg = JsonUtil.ToJson(message);
            //Log.sLog("Server " + msg);
            var type = (EMsgType) opcode;
            switch (type) {
                case EMsgType.JoinRoom:
                    OnPlayerConnect(session, message);
                    break;
                case EMsgType.QuitRoom:
                    OnPlayerQuit(session, message);
                    break;
                case EMsgType.PlayerInput:
                    OnPlayerInput(session, message);
                    break;
                case EMsgType.HashCode:
                    OnPlayerHashCode(session, message);
                    break;
            }
        }

        public void Update(){
            var now = DateTime.Now;
            _deltaTime = (now - _lastUpdateTimeStamp).TotalSeconds;
            if(_deltaTime > UpdateInterval){
                _lastUpdateTimeStamp = now;
                _timeSinceStartUp = (now - _startUpTimeStamp).TotalSeconds;
                DoUpdate();
            }
        }

        public void DoUpdate(){
            var fDeltaTime = (float) _deltaTime;
            var fTimeSinceStartUp = (float) _timeSinceStartUp;
            _room?.DoUpdate(fTimeSinceStartUp, fDeltaTime);
        }

        void OnPlayerConnect(Session session, IMessage message) {
            var msg = message as Msg_JoinRoom;
            msg.name = msg.name + idCounter;
            var name = msg.name;
            if(_name2Player.TryGetValue(name,  out var val)) {
                return;
            }
            var info = new PlayerServerInfo();
            info.id = idCounter++;
            info.name = name;
            _name2Player[name] = info;
            _id2Player[info.id] = info;
            _id2Session[info.id] = session;
            session.BindInfo = info;
            curCount++;
            if(curCount >= Room.MaxPlayerCount) {
                _room = new Room();
                _room.Init(0);
                foreach(var player in _id2Player.Values) {
                    _room.OnPlayerJoin(_id2Session[player.id], player);
                }
                OnGameStart(_room);
            }
            Debug.Log("OnPlayerConnect count:" + curCount + " " + JsonUtil.ToJson(msg));
        }

        void OnPlayerQuit(Session session, IMessage message) {
            Debug.Log("OnPlayerQuit count:" + curCount);
            var Player = session.GetBindInfo<PlayerServerInfo>();
            if(Player == null) {
                return;
            }
            _id2Player.Remove(Player.id);
            _name2Player.Remove(Player.name);
            _id2Session.Remove(Player.id);
            curCount--;
            if (curCount == 0) {
                _room = null;
            }

        }

        void OnPlayerInput(Session session, IMessage message) {
            var msg = message as Msg_PlayerInput;
            var player = session.GetBindInfo<PlayerServerInfo>();
            _room?.OnPlayerInput(player.id, msg);
        }
        void OnPlayerHashCode(Session session, IMessage message) {
            var msg = message as Msg_HashCode;
            var player = session.GetBindInfo<PlayerServerInfo>();
            _room?.OnPlayerHashCode(player.id, msg);
        }

        void OnGameStart(Room room) {
            if (room.IsRunning) {
                return;
            }
            room.OnGameStart();
        }

    }
}