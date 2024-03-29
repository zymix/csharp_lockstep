using System;
using System.Threading;
using Lockstep.Logging;
using Lockstep.Network;

namespace Lockstep.FakeServer{
    public class ServerLauncher{
        private static Server server;
        public static void Main(){
            OneThreadSynchronizationContext context = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            Debug.Log("Main start");
            try{
                DoAwake();
                while (true) {
                    try {
                        Thread.Sleep(3);
                        context.Update();
                        server.Update();
                    }
                    catch (ThreadAbortException e) {
                        return;
                    }
                    catch (Exception e) {
                        Log.Error(e.ToString());
                    }
                }
            }catch(ThreadAbortException e){
                return;
            }catch(Exception e){
                Log.Error(e.ToString());
            }
        }

        static void DoAwake(){
            server = new Server();
            server.Start();
        }
    }
}