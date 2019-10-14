﻿using System;
using Lockstep.Logic;
using UnityEngine;
using System.Collections.Generic;

namespace LockstepTutorial {
    public class ResourceManager : UnityBaseManager{
        public static ResourceManager Instance { get; private set; }
        [HideInInspector] public GameConfig config;
        public string configPath = "GameConfig";
        public string pathPrefix = "Prefabs/";
        private Dictionary<int, GameObject> _id2Prefab = new Dictionary<int, GameObject>();

        public override void DoAwake() {
            Instance = this;
            config = Resources.Load<GameConfig>(configPath);
        }

        public static GameObject LoadPrefab(int id) {
            return Instance._LoadPrefab(id);
        }

        public static PlayerConfig GetPlayerConfig(int prefabId) {
            return Instance.config.GetPlayerConfig(prefabId);
        }
        public static EnemyConfig GetEnemyConfig(int prefabId) {
            return Instance.config.GetEnemyConfig(prefabId-10);
        }

        private GameObject _LoadPrefab(int id) {
            if(_id2Prefab.TryGetValue(id, out var val)) {
                return val;
            }
            if (id < 10) {
                var config = this.config.GetPlayerConfig(id);
                var prefab = (GameObject)Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                return prefab;
            }
            if (id >= 10) {
                var config = this.config.GetEnemyConfig(id-10);
                var prefab = (GameObject)Resources.Load(pathPrefix + config.prefabPath);
                _id2Prefab[id] = prefab;
                return prefab;
            }
            return null;
        }
    }
}