﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class AOIComponent : Component, IAwake
    {
        public static AOIComponent Instance { get; set; }

        public void Awake()
        {
            Instance = this;
        }

        private Dictionary<int, AOIMap> Maps { get; set; } = new Dictionary<int, AOIMap>();

        public AOIMap GetMap(int mapID)
        {
            if (Maps.ContainsKey(mapID))
                return Maps[mapID];
            else
                return null;
        }

        public void CreateMap(int mapID, int gridSize)
        {
            if (Maps.ContainsKey(mapID))
            {
                Log.Error("此地图已存在");
                return;
            }
            Maps.Add(mapID, EntityFactory.CreatWithID<AOIMap, int>(mapID, gridSize));
        }

        public void AddMap(int mapID, AOIEntity entity)
        {
            if (!Maps.ContainsKey(mapID))
            {
                Log.Error("此地图不存在");
                return;
            }
            Maps[mapID].Add(entity);
        }

        public void AddMap(int mapID, AOIEntity entity, Vector3 position)
        {
            if (!Maps.ContainsKey(mapID))
            {
                Log.Error("此地图不存在");
                return;
            }
            Maps[mapID].Add(entity, position);
        }
    }
}
