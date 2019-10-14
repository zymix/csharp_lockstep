using System.Collections.Generic;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {
    class EnemyManager : UnityBaseManager {
        public List<Spawner> spawners = new List<Spawner>();
        public static EnemyManager Instance { get; private set; }
        public List<Enemy> allEnemy = new List<Enemy>();

        public static int maxCount = 1;
        private static int curCount = 0;
        private static int enemyID = 0;

        public override void DoAwake() {
            Instance = this;
        }

        public override void DoStart() {
            foreach(var spawner in spawners) {
                spawner.OnSpawnEvent += OnSpawn;
                spawner.DoStart();
            }
        }

        private void OnSpawn(int prefabId, LVector3 position) {
            if (curCount >= maxCount) {
                return;
            }
            ++curCount;
            var entity = InstantiateEntity(prefabId, position);
            Instance.AddEnemy(entity as Enemy);
        }

        public void AddEnemy(Enemy enemy) {
            allEnemy.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy) {
            allEnemy.Remove(enemy);
        }

        public static BaseEntity InstantiateEntity(int prefabId, LVector3 position) {
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.GetEnemyConfig(prefabId);
            Debug.Trace("createEnemy");
            var entity = new Enemy();
            var obj = UnityEntityService.CreateEntity(entity, prefabId, position, prefab, config);
            obj.name = obj.name + enemyID++;
            return entity;
        }
    }
}
;