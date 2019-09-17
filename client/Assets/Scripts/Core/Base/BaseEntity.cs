using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic{
    public class BaseEntity : BaseLifeCycle, IEntity, ILPTriggerEventHandler{
        public static int idCounter{get;private set;}
        public CRigidbody rigidbody = new CRigidbody();
        public CTransform2D transform = new CTransform2D();
        protected List<BaseComponent> allComponents = new List<BaseComponent>();
        public int entityID;
        public int prefabID;
        public object engineTransform;
        public LFloat speed = 5;
        public int currentHealth;

        public BaseEntity(){
            Debug.Trace("BaseEntity " + idCounter.ToString(), true);
            entityID = ++idCounter;
            rigidbody.transform2D = transform;
        }

        protected void RegisterComponent(BaseComponent comp){
            allComponents.Add(comp);
            comp.BindEntity(this);
        }

        public override void DoAwake(){
            foreach (var comp in allComponents){
                comp.DoAwake();
            }
        }

        public override void DoDestroy(){
            foreach (var comp in allComponents){
                comp.DoDestroy();
            }
        }

        public override void DoStart(){
            foreach (var comp in allComponents){
                comp.DoStart();
            }
        }

        public override void DoUpdate(LFloat deltaTime){
            foreach (var comp in allComponents){
                comp.DoUpdate(deltaTime);
            }
        }
        public virtual void OnLPTriggerEnter(ColliderProxy other) { }
        public virtual void OnLPTriggerExit(ColliderProxy other) { }
        public virtual void OnLPTriggerStay(ColliderProxy other) { }
    }
}