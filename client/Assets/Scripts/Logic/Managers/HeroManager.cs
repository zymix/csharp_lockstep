using System;

using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    internal class HeroManager {
        public static GameObject InstantiateEntity(Player entity, int prefabId, LVector3 position) {
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.GetPlayerConfig(prefabId);
            var obj = UnityEntityService.CreateEntity(entity, prefabId, position, prefab, config);
            return obj;
        }
    }
}