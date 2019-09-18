using Lockstep.Logic;

namespace LockstepTutorial {
    public abstract class BasePlayerComponent: BaseComponent{
        public Player player;
        public PlayerInput input => player.inputAgent;

        public override void BindEntity(BaseEntity entity) {
            base.BindEntity(entity);
            player = (Player)entity;
        }
    }
}