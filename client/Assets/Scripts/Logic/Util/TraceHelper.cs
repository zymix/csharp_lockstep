using Lockstep.Logging;
using Lockstep.Logic;
using System.Text;

namespace LockstepTutorial {
    public class TraceHelper {
        private static TraceHelper instance = new TraceHelper();
        StringBuilder dumpBuild = new StringBuilder();
        
        public static void TraceFrameState() {
            instance._TranceFrameState();
        }

        private void _TranceFrameState() {
            dumpBuild.AppendLine("Tick: " + GameManager.Instance.curFrameIdx);
            foreach(var input in GameManager.Instance.curFrameInput.inputs) {
                DumpInput(input);
            }
            foreach (var entity in GameManager.allPlayers) {
                DumpEntity(entity);
            }
            foreach (var entity in EnemyManager.Instance.allEnemy) {
                //dumpSb.Append(" " + entity.timer);
                dumpBuild.Append(" " + (entity.target == null ? "" : entity.target.entityID.ToString()));
                DumpEntity(entity);
            }

            Debug.Trace(dumpBuild.ToString(), true);
            dumpBuild.Clear();
        }

        public void DumpInput(PlayerInput input) {
            dumpBuild.Append("    ");
            dumpBuild.Append(" skillId:" + input.skillId);
            dumpBuild.Append(" " + input.mousePos);
            dumpBuild.Append(" " + input.inputUV);
            dumpBuild.Append(" " + input.isInputFire);
            dumpBuild.Append(" " + input.isSpeedUp);
            dumpBuild.AppendLine();
        }

        public void DumpEntity(BaseEntity entity) {
            dumpBuild.Append("    ");
            dumpBuild.Append(" " + entity.entityID);
            dumpBuild.Append(" " + entity.transform.Pos3);
            dumpBuild.Append(" " + entity.transform.deg);
            dumpBuild.AppendLine();
        }
    }
}
