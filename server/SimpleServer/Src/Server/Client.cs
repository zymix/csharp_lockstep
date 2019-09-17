using Lockstep.Network;
namespace Lockstep.FakeServer{
    public class Client:IMessageDispacher{
        private NetOuterProxy net = new NetOuterProxy();
        public Session session;
        private int count = 0;
        public int id;

        public void Start(){
            net.Awake(NetworkProtocol.TCP);
            net.MessageDispatcher = this;
            net.MessagePacker = MessagePacker.Instance;
            session = net.Create(Server.serverIpPoint);
            session.Start();
        }
    }
}