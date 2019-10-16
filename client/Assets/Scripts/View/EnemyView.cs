using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    public class EnemyView:MonoBehaviour, IEnemyView {
        public Enemy owner;
        private bool _isSinking = false;
        private float _sinkSpeed = 1.0f;
        public void BindEntity(BaseEntity entity) {
            owner = entity as Enemy;
            owner.eventHandler = this;
        }

        void Awake() { }
        void Update() {
            if (!_isSinking) {
                var pos = owner.transform.Pos3.ToVector3();
                transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
                var deg = owner.transform.deg.ToFloat();
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
            }
        }
    }
}
