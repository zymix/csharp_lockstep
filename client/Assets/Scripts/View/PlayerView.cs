using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    public partial class PlayerView : MonoBehaviour, IPlayerView {
        public Player owner;
        public int currentHealth => owner.currentHealth;
        public static LFloat minRunSpd = new LFloat(1);
        public static LFloat minFastRunSpd = new LFloat(7);

        public void Animating(bool isIdle) {
        }

        public void BindEntity(BaseEntity entity) {
            owner = entity as Player;
            var config = ResourceManager.GetPlayerConfig(owner.prefabID);
            var go = transform.Find(config.attackTransName);
            transform.position = owner.transform.Pos3.ToVector3();

            owner.eventHandler = this;
        }
        private void Update() {
            var pos = owner.transform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = owner.transform.deg.ToFloat();
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }
        private void Awake() {}

        public void OnDead() {
            DisableEffects();
        }
        public void DisableEffects() { }

        public void TakeDamage(int amount, LVector3 hitPoint) { }
    }
}
