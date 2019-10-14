﻿using System;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    public class HeroManager : UnityBaseManager {
        public static GameObject InstantiateEntity(Player entity, int prefabId, LVector3 position) {
            var prefab = ResourceManager.LoadPrefab(prefabId);
            object config = ResourceManager.GetPlayerConfig(prefabId);
            var obj = UnityEntityService.CreateEntity(entity, prefabId, position, prefab, config);
            return obj;
        }

        public override void DoUpdate(LFloat deltaTime) {
            foreach(var player in GameManager.allPlayers) {
                player.DoUpdate(deltaTime);
            }
        }
    }
}