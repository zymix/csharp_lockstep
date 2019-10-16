using UnityEngine;

namespace LockstepTutorial {
    public class CameraFollow : MonoBehaviour {
        Vector3 offset;
        public float smoothing = 5f;

        public Transform _target;
        public Transform Target {
            get => _target;
            set {
                _target = value;
                if (_target != null) {
                    offset = transform.position - _target.position;
                }
            }
        }
        private void FixedUpdate() {
            if(_target == null) {
                Target = GameManager.MyPlayerTrans;
            }
            if (_target == null) {
                return;
            }
            Vector3 targetCamPos = Target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 0.1f);
        }
    }
}