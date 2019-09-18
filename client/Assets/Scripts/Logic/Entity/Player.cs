
using Lockstep.Logic;

namespace LockstepTutorial {
    public partial class Player:BaseEntity {
        public int localId;
        public IPlayerView eventHandler;
        public PlayerInput inputAgent = new PlayerInput();
        public CMover cmover = new CMover();
        
        public Player() {
            RegisterComponent(cmover);
        }
    }
}
