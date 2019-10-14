using Lockstep.Collision2D;
using Lockstep.Logic;
namespace LockstepTutorial {
    public class Enemy : BaseEntity {
        public IEnemyView eventHandler;
        public BaseEntity target;
    }
}