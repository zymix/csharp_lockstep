﻿using Lockstep.Collision2D;
using Lockstep.Math;

namespace LockstepTutorial {
    public partial class CMover:BasePlayerComponent {
        static LFloat sqrStopDist = new LFloat(true, 40);
        public LFloat speed => entity.speed;
        public bool hasReachTarget = false;
        public bool needMove = true;

        public override void DoUpdate(LFloat deltaTime) {
            var needChase = input.inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase) {
                var dir = input.inputUV.normalized;
                transform.pos = transform.pos + dir * speed * deltaTime;
                var targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, 360 * deltaTime, out var hasReachDeg);
            }
            hasReachTarget = !needChase;
        }
    }
}
