using Lockstep.Collision2D;

namespace Lockstep.Util {
    public static class TransformHashCodeExtension {
        public static int GetHash(this CTransform2D val) {
            return val.Pos3.GetHash() * 13 + val.deg.GetHash() * 31;
        }
    }
}
