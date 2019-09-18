using Lockstep.Network;
using LockstepTutorial;
using System;
using System.Net;

namespace Lockstep.Logic {
    public class NetClient : IMessageDispatcher{
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);
        private NetOuterProxy net = new NetOuterProxy();
        public Session session;
        private int count = 0;
        public int id;

        public void Start() {
            net.Awake(NetworkProtocol.TCP);
            net.MessageDispatcher = this;
            net.MessagePacker = MessagePacker.Instance;
            session = net.Create(serverIpPoint);
            session.Start();
        }

        public void Dispatch(Session session, Packet packet) {
            ushort opcode = packet.Opcode();
            var message = session.Network.MessagePacker.DeserializeFrom(opcode, packet.Bytes,
                Packet.Index, packet.Length - Packet.Index) as IMessage;
            var type = (EMsgType)opcode;
            switch (type) {
                case EMsgType.FrameInput:
                    OnFrameInput(session, message);break;
                case EMsgType.StartGame:
                    OnStartGame(session, message);break;
            }

        }

        public void OnStartGame(Session session, IMessage message) {
            var msg = message as Msg_StartGame;
            GameManager.StartGame(msg);
        }

        public void OnFrameInput(Session session, IMessage message) {
            var msg = message as Msg_FrameInput;
            GameManager.PushFrameInput(msg.input);
        }
        public void Send(IMessage msg) {
            session.Send(msg);
        }
    }
}